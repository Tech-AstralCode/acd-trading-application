using System.Text.Json;
using Application.Abstractions;
using Application.Services;
using Application.Strategies;
using Domain.Signals;
using Domain.Strategies;
using Domain.Trading;

namespace Application.Services;

public interface IStrategyRunner
{ Task<int> RunOnceAsync(string underlying, CancellationToken ct = default); }

public sealed class StrategyRunner : IStrategyRunner
{
    private readonly IStrategyRegistry _registry;
    private readonly IChainQueryService _chainQ;
    private readonly ICandleQueryService _candleQ;
    private readonly ITradingRepository _repo;

    public StrategyRunner(IStrategyRegistry registry, IChainQueryService chainQ, ICandleQueryService candleQ, ITradingRepository repo)
    { _registry = registry; _chainQ = chainQ; _candleQ = candleQ; _repo = repo; }

    public async Task<int> RunOnceAsync(string underlying, CancellationToken ct = default)
    {
        var chain = _chainQ.GetFromCache(underlying);
        var candles = _candleQ.GetFromCache(underlying, "1m");
        if (chain is null || candles.Count == 0) return 0;

        var strategies = await _repo.GetEnabledStrategiesAsync(ct);
        int inserted = 0;
        foreach (var s in strategies)
        {
            var impl = _registry.Resolve(s.Name);
            if (impl is null) continue;

            var dict = JsonSerializer.Deserialize<Dictionary<string,string>>(s.ConfigJson) ?? new();
            var cfg = new StrategyConfig(dict);

            var ctx = new MarketContext { Underlying = underlying, Candles = candles, Chain = chain, Now = DateTimeOffset.Now };
            var sigs = await impl.GenerateAsync(ctx, cfg, ct);

            foreach (var sig in sigs)
            {
                var exists = await _repo.RecentSignalExistsAsync(sig.OptionSymbol, TimeSpan.FromMinutes(2), ct);
                if (exists) continue;

                await _repo.AddSignalAsync(new Signal
                {
                    Id = Guid.NewGuid(),
                    Time = sig.Time,
                    Underlying = sig.Underlying,
                    Side = sig.Side,
                    Strike = sig.Strike,
                    Expiry = sig.Expiry,
                    Confidence = sig.Confidence,
                    OptionSymbol = sig.OptionSymbol,
                    ReasonJson = sig.ReasonJson,
                    StrategyId = s.Id
                }, ct);
                inserted++;
            }
        }
        if (inserted > 0) await _repo.SaveChangesAsync(ct);
        return inserted;
    }
}
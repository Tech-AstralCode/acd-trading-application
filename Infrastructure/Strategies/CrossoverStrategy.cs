using System.Text.Json;
using Application.Indicators;
using Domain.Strategies;
using Domain.Trading;

namespace Infrastructure.Strategies;

public sealed class CrossoverStrategy : IStrategy
{
    public string Name => "Crossover";

    public Task<IReadOnlyList<StrategySignal>> GenerateAsync(MarketContext ctx, StrategyConfig cfg, CancellationToken ct)
    {
        var p = cfg.Params;
        var fast = int.Parse(p.GetValueOrDefault("FastEMA", "9"));
        var slow = int.Parse(p.GetValueOrDefault("SlowEMA", "21"));
        var minMom = double.Parse(p.GetValueOrDefault("MinMomentum", "0.20")); // require momentum
        var bias = p.GetValueOrDefault("StrikeBias", "ATM");

        if (ctx.Candles.Count < Math.Max(fast, slow) + 2)
            return Task.FromResult<IReadOnlyList<StrategySignal>>(Array.Empty<StrategySignal>());

        var closes = ctx.Candles.Select(c => c.Close).ToList();
        var emaF = Application.Indicators.TA.Ema(closes, fast);
        var emaS = Application.Indicators.TA.Ema(closes, slow);
        var mom = Application.Indicators.TA.Momentum(closes, 10);
        int i = closes.Count - 1;

        bool bullCross = emaF[i] > emaS[i] && emaF[i - 1] <= emaS[i - 1] && mom[i] > minMom;
        bool bearCross = emaF[i] < emaS[i] && emaF[i - 1] >= emaS[i - 1] && mom[i] < -minMom;

        if (!bullCross && !bearCross)
            return Task.FromResult<IReadOnlyList<StrategySignal>>(Array.Empty<StrategySignal>());

        var prefCall = bullCross; // CALL on bull, PUT on bear
        var offset = bias switch { "ITM" => (prefCall ? -1 : +1), "OTM" => (prefCall ? +1 : -1), _ => 0 };
        var pick = ctx.PickAtm(callPreference: prefCall, strikeOffset: offset);
        if (pick is null) return Task.FromResult<IReadOnlyList<StrategySignal>>(Array.Empty<StrategySignal>());

        var (row, strike) = pick.Value;
        var conf = (decimal)Math.Min(0.9, 0.55 + Math.Abs(mom[i]));
        var reason = System.Text.Json.JsonSerializer.Serialize(new { ema = $"{fast}/{slow}", momentum = mom[i], bias });

        var sig = new StrategySignal(
            Time: ctx.Now,                 // Runner sets UtcNow
            Underlying: ctx.Underlying,
            Side: prefCall ? Domain.Signals.TradeSide.Call : Domain.Signals.TradeSide.Put,
            Strike: strike,
            Expiry: row.Expiry,
            Confidence: conf,
            OptionSymbol: row.Symbol,
            ReasonJson: reason
        );
        return Task.FromResult<IReadOnlyList<StrategySignal>>(new[] { sig });
    }
}
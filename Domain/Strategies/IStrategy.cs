using Domain.Trading;

namespace Domain.Strategies;

public interface IStrategy
{
    string Name { get; }
    Task<IReadOnlyList<StrategySignal>> GenerateAsync(MarketContext ctx, StrategyConfig cfg, CancellationToken ct);
}

public sealed record StrategyConfig(Dictionary<string, string> Params);

public sealed record StrategySignal(
    DateTimeOffset Time,
    string Underlying,
    Domain.Signals.TradeSide Side,
    int Strike,
    DateOnly Expiry,
    decimal Confidence,
    string OptionSymbol,
    string ReasonJson
);
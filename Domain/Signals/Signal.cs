using System;

namespace Domain.Signals;

public enum TradeSide { Call = 1, Put = 2 }

public sealed class Signal
{
    public Guid Id { get; set; }
    public DateTimeOffset Time { get; set; }
    public string Underlying { get; set; } = string.Empty;
    public TradeSide Side { get; set; }
    public int Strike { get; set; }
    public DateOnly Expiry { get; set; }
    public decimal Confidence { get; set; }
    public string OptionSymbol { get; set; } = string.Empty;
    public string ReasonJson { get; set; } = "{}";
    public Guid? StrategyId { get; set; }
}

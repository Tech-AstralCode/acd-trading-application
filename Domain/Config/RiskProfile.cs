using System;

namespace Domain.Config;

public sealed class RiskProfile
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "Conservative";
    public decimal PerTradeRiskPct { get; set; } = 0.75m; // % of equity
    public decimal DailyLossCapR { get; set; } = 2m; // in R multiples
    public int MaxConcurrent { get; set; } = 1;
    public int SlippageBps { get; set; } = 8; // basis points
    public TimeOnly NoEntryAfter { get; set; } = new(15, 10);
}

using System;

namespace Domain.Strategies;

public enum StrategyMode { Manual = 0, Confirm = 1, Auto = 2 }

public sealed class Strategy
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "Crossover";
    public bool Enabled { get; set; } = true;
    public StrategyMode Mode { get; set; } = StrategyMode.Manual;
    public decimal AiThreshold { get; set; } = 0.62m;
    public string ConfigJson { get; set; } = "{\"FastEMA\":\"9\",\"SlowEMA\":\"21\",\"MinMomentum\":\"0.25\",\"StrikeBias\":\"ATM\"}";
    public Guid? RiskProfileId { get; set; }
}

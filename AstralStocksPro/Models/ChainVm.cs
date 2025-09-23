using System;
using Domain.Trading;

namespace AstralStocksPro.Models;

public sealed class ChainVm
{
    public string Underlying { get; set; } = string.Empty;
    public DateTimeOffset Time { get; set; }
    public decimal Spot { get; set; }
    public List<StrikeRowVm> Strikes { get; set; } = new();
    public List<ChainRowVm> Rows { get; set; } = new(); // unused in this view but kept for future
}

public sealed class StrikeRowVm
{
    public int Strike { get; set; }
    public DateOnly Expiry { get; set; }
    public ChainRow? Call { get; set; }
    public ChainRow? Put { get; set; }
    public bool IsAtm { get; set; }
}

public sealed class ChainRowVm
{
    public string Symbol { get; set; } = string.Empty;
    public bool IsCall { get; set; }
    public int Strike { get; set; }
    public DateOnly Expiry { get; set; }
    public decimal Ltp { get; set; }
    public decimal Bid { get; set; }
    public decimal Ask { get; set; }
    public int Oi { get; set; }
    public int OiChange { get; set; }
    public decimal? Iv { get; set; }
}

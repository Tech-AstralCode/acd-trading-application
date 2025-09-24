namespace Domain.Trading;

public sealed class MarketContext
{
    public required string Underlying { get; init; }
    public required IReadOnlyList<Candle> Candles { get; init; }
    public required ChainSnapshot Chain { get; init; }
    public required DateTimeOffset Now { get; init; }

    public (ChainRow row, int strike)? PickAtm(bool callPreference, int strikeOffset = 0)
    {
        var spot = EstimateSpotFromChain(Chain);
        var atm = Chain.AtmStrike(spot) ?? 0;
        var targetStrike = atm + strikeOffset * 50; // assumes 50 steps; adjust if needed
        var sideRows = Chain.Rows.Where(r => r.Strike == targetStrike).ToList();
        var chosen = sideRows.FirstOrDefault(r => r.IsCall == callPreference) ?? sideRows.FirstOrDefault();
        return chosen is null ? null : (chosen, targetStrike);
    }

    private static decimal EstimateSpotFromChain(ChainSnapshot snap)
    {
        var byStrike = snap.Rows.GroupBy(r => r.Strike).OrderBy(g => g.Key).ToList();
        var mid = byStrike.Skip(Math.Max(0, byStrike.Count/2 - 1)).FirstOrDefault()?.Key ?? 0;
        return mid;
    }
}
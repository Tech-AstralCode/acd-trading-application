using System;

namespace Domain.Trading;

public sealed record ChainRow(
    string Symbol,
    bool IsCall,
    int Strike,
    DateOnly Expiry,
    decimal Ltp,
    decimal Bid,
    decimal Ask,
    int Oi,
    int OiChange,
    decimal? Iv
);

public sealed record ChainSnapshot(
        string Underlying,
        DateTimeOffset Time,
        IReadOnlyList<ChainRow> Rows
    )
{
    public int? AtmStrike(decimal underlyingSpot, int strikeStep = 50)
    {
        // Round to nearest step
        var rounded = (int)(Math.Round(underlyingSpot / strikeStep, MidpointRounding.AwayFromZero) * strikeStep);
        return Rows.Any(r => r.Strike == rounded) ? rounded : Rows
        .Select(r => r.Strike)
        .OrderBy(s => Math.Abs(s - rounded))
        .FirstOrDefault();
    }
}

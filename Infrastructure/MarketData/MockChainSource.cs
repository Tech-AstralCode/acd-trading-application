using System.Text.Json;
using Domain.Trading;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.MarketData;

public sealed class MockChainSource : IChainSource
{
    private readonly string _basePath;

    public MockChainSource(IConfiguration config)
    {
        _basePath = config["Data:MockChainPath"] ?? "App_Data";
    }

    public async Task<ChainSnapshot?> GetSnapshotAsync(string underlying, CancellationToken ct = default)
    {
        var file = Path.Combine(_basePath, $"chain_{underlying.ToUpperInvariant()}.json");
        if (!File.Exists(file)) return null;
        await using var fs = File.OpenRead(file);
        var dto = await JsonSerializer.DeserializeAsync<ChainFileDto>(fs, cancellationToken: ct);
        if (dto == null) return null;

        var rows = dto.rows.Select(r => new ChainRow(
        Symbol: r.symbol,
        IsCall: r.isCall,
        Strike: r.strike,
        Expiry: DateOnly.Parse(r.expiry),
        Ltp: r.ltp,
        Bid: r.bid,
        Ask: r.ask,
        Oi: r.oi,
        OiChange: r.oiChange,
        Iv: r.iv
        )).ToList();

        return new ChainSnapshot(dto.underlying, dto.time, rows);
    }

    private sealed class ChainFileDto
    {
        public string underlying { get; set; } = string.Empty;
        public DateTimeOffset time { get; set; }
        public List<Row> rows { get; set; } = new();
        public sealed class Row
        {
            public string symbol { get; set; } = string.Empty;
            public bool isCall { get; set; }
            public int strike { get; set; }
            public string expiry { get; set; } = string.Empty; // yyyy-MM-dd
            public decimal ltp { get; set; }
            public decimal bid { get; set; }
            public decimal ask { get; set; }
            public int oi { get; set; }
            public int oiChange { get; set; }
            public decimal? iv { get; set; }
        }
    }
}
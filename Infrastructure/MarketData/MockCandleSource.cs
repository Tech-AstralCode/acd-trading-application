using System.Text.Json;
using Domain.Trading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.MarketData;

public sealed class MockCandleSource : ICandleSource
{
    private readonly string _basePath;
    private readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };
    public MockCandleSource(IConfiguration cfg,IHostEnvironment env)
    {
        var configured = cfg["Data:MockCandlePath"] ?? "App_Data";
        _basePath = Path.IsPathRooted(configured) ? configured : Path.Combine(env.ContentRootPath, configured);
    
    }

    public async Task<IReadOnlyList<Candle>> GetCandlesAsync(string underlying, string interval, int limit, CancellationToken ct = default)
    {
        var file = Path.Combine(_basePath, $"candles_{underlying.ToUpperInvariant()}_{interval}.json");
        if (!File.Exists(file)) return Array.Empty<Candle>();
        await using var fs = File.OpenRead(file);
        var items = await JsonSerializer.DeserializeAsync<List<CandleDto>>(fs, _opts, ct) ?? new();
        var ordered = items.OrderBy(i => i.time).Select(i => new Candle(i.time, i.open, i.high, i.low, i.close, i.volume)).ToList();
        if (ordered.Count > limit) ordered = ordered.Skip(Math.Max(0, ordered.Count - limit)).ToList();
        return ordered;
    }

    private sealed class CandleDto
    {
        public DateTimeOffset time { get; set; }
        public decimal open { get; set; }
        public decimal high { get; set; }
        public decimal low  { get; set; }
        public decimal close { get; set; }
        public long volume { get; set; }
    }
}
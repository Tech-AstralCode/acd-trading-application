using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.MarketData;

public sealed class CandleCacheService : BackgroundService
{
    private readonly ICandleSource _source;
    private readonly IMemoryCache _cache;
    private readonly string[] _underlyings;
    private readonly string? _interval;
    private readonly TimeSpan _delay;

    public CandleCacheService(ICandleSource source, IMemoryCache cache, IConfiguration cfg)
    {
        _source = source; _cache = cache;
        _underlyings = cfg.GetSection("Trading:Underlyings").Get<string[]>() ?? new[] {"NIFTY"};
        _interval = cfg.GetValue<string>("Trading:CandleInterval", "1m") ?? "1m";
        _delay = TimeSpan.FromSeconds(cfg.GetValue("Trading:PollingSeconds", 5));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var u in _underlyings)
            {
                try
                {
                    var candles = await _source.GetCandlesAsync(u, _interval, 500, stoppingToken);
                    _cache.Set($"candles:{u}:{_interval}", candles, TimeSpan.FromSeconds(30));
                }
                catch { /* log in production */ }
            }
            await Task.Delay(_delay, stoppingToken);
        }
    }
}
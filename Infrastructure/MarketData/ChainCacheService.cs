using Domain.Trading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.MarketData;

public sealed class ChainCacheService : BackgroundService
{
    private readonly IChainSource _source;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ChainCacheService> _log;
    private readonly string[] _underlyings;
    private readonly TimeSpan _interval;

    public ChainCacheService(IChainSource source, IMemoryCache cache, ILogger<ChainCacheService> log, IConfiguration cfg)
    {
        _source = source; _cache = cache; _log = log;
        _underlyings = cfg.GetSection("Trading:Underlyings").Get<string[]>() ?? new[] { "NIFTY" };
        _interval = TimeSpan.FromSeconds(cfg.GetValue("Trading:PollingSeconds", 5));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("ChainCacheService started for {Count} underlyings", _underlyings.Length);
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var u in _underlyings)
            {
                try
                {
                    var snap = await _source.GetSnapshotAsync(u, stoppingToken);
                    if (snap != null)
                        _cache.Set($"chain:{u.ToUpperInvariant()}", snap, TimeSpan.FromSeconds(15));
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Error refreshing chain {Underlying}", u);
                }
            }
            await Task.Delay(_interval, stoppingToken);
        }
    }
}
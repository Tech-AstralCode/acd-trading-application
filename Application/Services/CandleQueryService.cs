using Domain.Trading;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services;

public interface ICandleQueryService
{
    IReadOnlyList<Candle> GetFromCache(string underlying, string interval);
}

public sealed class CandleQueryService : ICandleQueryService
{
    private readonly IMemoryCache _cache;
    public CandleQueryService(IMemoryCache cache) => _cache = cache;
    public IReadOnlyList<Candle> GetFromCache(string underlying, string interval)
        => _cache.TryGetValue($"candles:{underlying}:{interval}", out IReadOnlyList<Candle> c) ? c ?? Array.Empty<Candle>() : Array.Empty<Candle>();
}
using Domain.Trading;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services;

public interface IChainQueryService
{
    ChainSnapshot? GetFromCache(string underlying);
}

public sealed class ChainQueryService : IChainQueryService
{
    private readonly IMemoryCache _cache;
    public ChainQueryService(IMemoryCache cache) => _cache = cache;

    public ChainSnapshot? GetFromCache(string underlying)
    => _cache.TryGetValue($"chain:{underlying.ToUpperInvariant()}", out ChainSnapshot snap) ? snap : null;
}
using Domain.Trading;

namespace Infrastructure.MarketData;

public interface IChainSource
{
    Task<ChainSnapshot?> GetSnapshotAsync(string underlying, CancellationToken ct = default);
}
using Domain.Trading;

namespace Infrastructure.MarketData;

public interface ICandleSource
{ Task<IReadOnlyList<Candle>> GetCandlesAsync(string underlying, string interval, int limit, CancellationToken ct = default); }
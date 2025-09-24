using Domain.Signals;
using Domain.Strategies;

namespace Application.Abstractions;

public interface ITradingRepository
{
    Task<IReadOnlyList<Strategy>> GetEnabledStrategiesAsync(CancellationToken ct = default);
    Task<bool> RecentSignalExistsAsync(string optionSymbol, TimeSpan lookback, CancellationToken ct = default);
    Task<List<Signal>> GetLatestSignalsAsync(string? underlying, int take, CancellationToken ct = default);
    Task AddSignalAsync(Signal s, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
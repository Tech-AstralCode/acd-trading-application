using Application.Abstractions;
using Domain.Signals;
using Domain.Strategies;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class TradingRepository : ITradingRepository
{
    private readonly TradingDbContext _db;
    public TradingRepository(TradingDbContext db) => _db = db;

    public async Task<IReadOnlyList<Strategy>> GetEnabledStrategiesAsync(CancellationToken ct = default)
        => await _db.Strategies.AsNoTracking()
              .Where(s => s.Enabled)
              .ToListAsync(ct);

    public async Task<bool> RecentSignalExistsAsync(string optionSymbol, TimeSpan lookback, CancellationToken ct = default)
    {
        var since = DateTimeOffset.UtcNow - lookback;
        return await _db.Signals.AsNoTracking()
            .AnyAsync(x => x.OptionSymbol == optionSymbol && x.Time > since, ct);
    }

    public Task AddSignalAsync(Signal s, CancellationToken ct = default)
    {
        _db.Signals.Add(s);
        return Task.CompletedTask;
    }

    public async Task<List<Signal>> GetLatestSignalsAsync(string? underlying, int take, CancellationToken ct = default)
    {
        var q = _db.Signals.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(underlying))
            q = q.Where(x => x.Underlying == underlying);

        return await q
            .OrderByDescending(x => x.Time)
            .Take(take)
            .ToListAsync(ct);
    }
    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
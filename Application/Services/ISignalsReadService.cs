using System;
using Application.Abstractions;

namespace Application.Services;

public interface ISignalsReadService
{
    Task<IReadOnlyList<SignalRowDto>> GetLatestAsync(string? underlying, int take, CancellationToken ct = default);
}

public sealed class SignalRowDto
{
    public Guid Id { get; set; }
    public DateTimeOffset Time { get; set; }
    public string Underlying { get; set; } = "";
    public string Side { get; set; } = "";
    public int Strike { get; set; }
    public DateOnly Expiry { get; set; }
    public decimal Confidence { get; set; }
    public string OptionSymbol { get; set; } = "";
    public string ReasonJson { get; set; } = "{}";
}

public sealed class SignalsReadService : ISignalsReadService
{
    private readonly ITradingRepository _repo;
    public SignalsReadService(ITradingRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<SignalRowDto>> GetLatestAsync(string? underlying, int take, CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 1000);
        var items = await _repo.GetLatestSignalsAsync(underlying, take, ct);
        return items.Select(x => new SignalRowDto
        {
            Id = x.Id,
            Time = x.Time,
            Underlying = x.Underlying,
            Side = x.Side.ToString(),
            Strike = x.Strike,
            Expiry = x.Expiry,
            Confidence = x.Confidence,
            OptionSymbol = x.OptionSymbol,
            ReasonJson = x.ReasonJson
        }).ToList();
    }
}
using Application.Services;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AstralStocksPro.Controllers
{
    [Route("debug")]
    public sealed class DebugController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly IStrategyRunner _runner;
        private readonly TradingDbContext _db;

        public DebugController(IMemoryCache cache, IStrategyRunner runner, TradingDbContext db)
        { _cache = cache; _runner = runner; _db = db; }

        [HttpGet("caches")]
        public IActionResult Caches(string underlying = "NIFTY", string interval = "1m")
        {
            var k1 = $"candles:{underlying}:{interval}";
            var k2 = $"chain:{underlying.ToUpperInvariant()}";

            var candles = _cache.TryGetValue(k1, out IReadOnlyList<Domain.Trading.Candle> c) ? c : Array.Empty<Domain.Trading.Candle>();
            var chain = _cache.TryGetValue(k2, out Domain.Trading.ChainSnapshot snap) ? snap : null;

            return Json(new
            {
                candleKey = k1,
                candleCount = candles.Count,
                chainKey = k2,
                chainRows = snap?.Rows.Count ?? 0,
                now = DateTimeOffset.Now
            });
        }

        [HttpPost("run-once")]
        public async Task<IActionResult> RunOnce(string underlying = "NIFTY", CancellationToken ct = default)
        {
            var n = await _runner.RunOnceAsync(underlying, ct);
            var latest = _db.Signals.OrderByDescending(x => x.Time).Take(3).Select(x => new
            {
                x.Time,
                x.Underlying,
                x.OptionSymbol,
                x.Confidence
            }).ToList();
            return Json(new { inserted = n, latest });
        }

        [HttpGet("strategies")]
        public IActionResult Strategies()
        {
            var list = _db.Strategies.Select(s => new { s.Name, s.Enabled, s.ConfigJson }).ToList();
            return Json(list);
        }

    }
}

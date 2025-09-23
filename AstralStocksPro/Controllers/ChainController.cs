using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Domain.Trading;
using AstralStocksPro.Models;

namespace AstralStocksPro.Controllers
{
    public sealed class ChainController : Controller
    {
        private readonly IChainQueryService _query;
        public ChainController(IChainQueryService query) => _query = query;

        public IActionResult Index(string symbol = "NIFTY")
        {
            var snap = _query.GetFromCache(symbol);
            if (snap is null) return View(new ChainVm { Underlying = symbol, Rows = new List<ChainRowVm>(), Time = DateTimeOffset.Now });

            var spot = EstimateSpot(snap);
            var atm = snap.AtmStrike(spot) ?? 0;

            var grouped = snap.Rows
            .GroupBy(r => r.Strike)
            .OrderBy(g => g.Key)
            .Select(g => new StrikeRowVm
            {
                Strike = g.Key,
                Expiry = g.First().Expiry,
                Call = g.FirstOrDefault(x => x.IsCall),
                Put = g.FirstOrDefault(x => !x.IsCall),
                IsAtm = g.Key == atm
            }).ToList();
            var vm = new ChainVm
            {
                Underlying = snap.Underlying,
                Time = snap.Time,
                Spot = spot,
                Strikes = grouped
            };
            return View(vm);
        }

        private static decimal EstimateSpot(ChainSnapshot snap)
        {
            // crude: average of nearest ATM call/put mid-prices implied; for mock we just derive from nearest strike
            var avgStrike = snap.Rows.Select(r => r.Strike).GroupBy(x => x).OrderBy(g => Math.Abs(g.Key - 25000)).First().Key; // mock heuristic
            return avgStrike; // adjust to your mock; with real data you’ll read actual spot from an underlier quote endpoint
        }
    }
}

using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AstralStocksPro.Controllers
{
    public sealed class SignalsController : Controller
    {
        private readonly ISignalsReadService _svc;
        public SignalsController(ISignalsReadService svc) => _svc = svc;

        public async Task<IActionResult> Index(string? underlying = null, int take = 200, CancellationToken ct = default)
        {
            var items = await _svc.GetLatestAsync(underlying, take, ct);
            return View(items);
        }
    }
}

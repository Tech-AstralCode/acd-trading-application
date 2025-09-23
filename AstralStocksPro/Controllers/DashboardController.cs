using Microsoft.AspNetCore.Mvc;

namespace AstralStocksPro.Controllers
{
    public sealed class DashboardController : Controller
    {
        // GET: DashboardController
        public ActionResult Index()
        {
            return View();
        }

    }
}

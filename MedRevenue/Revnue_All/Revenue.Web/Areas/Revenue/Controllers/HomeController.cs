using Microsoft.AspNetCore.Mvc;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            ViewBag.CasesCount = 150;
            ViewBag.ProductsCount = 75;
            ViewBag.Revenue = 250000;
            return View();
        }
    }
}
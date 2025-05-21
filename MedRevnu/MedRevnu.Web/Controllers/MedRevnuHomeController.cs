using ATI.MedRevnu.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ATI.MedRevnu.Web.Controllers
{
    [Area("MedRevnu")]
    public class MedRevnuHomeController : Controller
    {
        private readonly ILogger<MedRevnuHomeController> _logger;

        public MedRevnuHomeController(ILogger<MedRevnuHomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

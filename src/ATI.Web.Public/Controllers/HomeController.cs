using Microsoft.AspNetCore.Mvc;
using ATI.Web.Controllers;

namespace ATI.Web.Public.Controllers
{
    public class HomeController : ATIControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
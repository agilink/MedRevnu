using Microsoft.AspNetCore.Mvc;
using ATI.Web.Controllers;

namespace ATI.Web.Public.Controllers
{
    public class AboutController : ATIControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
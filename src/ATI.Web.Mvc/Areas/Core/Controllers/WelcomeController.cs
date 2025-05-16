using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using ATI.Web.Controllers;

namespace ATI.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [AbpMvcAuthorize]
    public class WelcomeController : ATIControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
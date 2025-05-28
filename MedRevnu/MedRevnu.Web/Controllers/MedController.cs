using ATI.MedRevnu.Web.Models;
using ATI.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ATI.MedRevnu.Web.Controllers
{
    [Area("MedRevnu")]    
    public class MedController : ATIControllerBase
    {        
        public IActionResult Index()
        {
            return View();
        }      
    }
}

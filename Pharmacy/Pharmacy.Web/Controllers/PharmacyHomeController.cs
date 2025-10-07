using Abp.Runtime.Session;
using ATI.Web.Controllers;
using Microsoft.AspNetCore.Mvc;


namespace ATI.Pharmacy.Web.Controllers
{
    [Area("Pharmacy")]
    public class PharmacyHomeController : ATIControllerBase
    {
       
        public PharmacyHomeController()
        {
            
        }
        public ActionResult Index()
        {           
            return View();
        }

       
    }
}

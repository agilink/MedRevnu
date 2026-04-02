using ATI.Revenue.Application.ProcedureQuotas;
using ATI.Revenue.Application.ProcedureQuotas.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    public class ProcedureQuotasController : Controller
    {
        private readonly IProcedureQuotasAppService _procedureQuotasAppService;

        public ProcedureQuotasController(IProcedureQuotasAppService procedureQuotasAppService)
        {
            _procedureQuotasAppService = procedureQuotasAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(GetAllProcedureQuotasInput input)
        {
            var result = await _procedureQuotasAppService.GetAll(input);
            return Json(result);
        }
    }
}

using ATI.Revenue.Application.ProcedureTypes;
using ATI.Revenue.Application.ProcedureTypes.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using ATI.Authorization;
using ATI.Web.Controllers;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    [AbpMvcAuthorize(AppPermissions.Pages_Revenue)]
    public class ProcedureTypesController : ATIControllerBase
    {
        private readonly IProcedureTypesAppService _procedureTypesAppService;

        public ProcedureTypesController(IProcedureTypesAppService procedureTypesAppService)
        {
            _procedureTypesAppService = procedureTypesAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(GetAllProcedureTypesInput input)
        {
            var result = await _procedureTypesAppService.GetAll(input);
            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> GetAllActive()
        {
            var result = await _procedureTypesAppService.GetAllActiveProcedureTypes();
            return Json(result);
        }
    }
}

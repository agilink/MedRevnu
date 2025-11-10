using Abp.AspNetCore.Mvc.Authorization;
using Abp.Application.Services.Dto;
using ATI.Web.Controllers;
using ATI.Revenue.Application.Cases;
using ATI.Revenue.Application.Cases.Dtos;
using ATI.Revenue.Web.PageModel.Cases;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ATI.Web.Controllers;

namespace ATI.Revenue.Web.Controllers
{
    [Area("Revenue")]
    [AbpMvcAuthorize("Pages.Cases")]
    public class CasesController : ATIControllerBase
    {
        private readonly ICasesAppService _casesAppService;

        public CasesController(ICasesAppService casesAppService)
        {
            _casesAppService = casesAppService;
        }

        public IActionResult Index()
        {
            var viewModel = new CasesViewModel
            {
                FilterText = ""
            };
            return View(viewModel);
        }

        [AbpMvcAuthorize("Pages.Cases.Create", "Pages.Cases.Edit")]
        public async Task<PartialViewResult> CreateOrEditModal(int? id)
        {
            var viewModel = new CreateOrEditCaseModalViewModel();
            
            if (id.HasValue)
            {
                var output = await _casesAppService.GetCaseForEdit(new EntityDto<int> { Id = id.Value });
                viewModel.Case = output.Case;
            }
            else
            {
                viewModel.Case = new CreateOrEditCaseDto();
            }

            viewModel.IsEditMode = id.HasValue;
            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewCaseModal(int id)
        {
            var output = await _casesAppService.GetCaseForView(id);
            var viewModel = new ViewCaseModalViewModel
            {
                Case = output.Case
            };
            return PartialView("_ViewCaseModal", viewModel);
        }
    }
}
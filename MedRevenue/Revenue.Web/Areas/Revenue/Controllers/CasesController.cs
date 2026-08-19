using Microsoft.AspNetCore.Mvc;
using ATI.Revenue.Application.Cases;
using ATI.Revenue.Application.Cases.Dtos;
using ATI.Revenue.Application.ProcedureTypes;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using ATI.Admin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ATI.Web.Controllers;
using ATI.Revenue.Web.PageModel.Cases;
using System;
using Abp.AspNetCore.Mvc.Authorization;
using ATI.Authorization;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    [AbpMvcAuthorize(AppPermissions.Pages_Revenue_Cases)]
    public class CasesController : ATIControllerBase
    {
        private readonly ICasesAppService _casesAppService;
        private readonly IProcedureTypesAppService _procedureTypesAppService;
        private readonly IRepository<Facility, int> _facilityRepository;
        private readonly IRepository<Personnel, int> _personnelRepository;

        public CasesController(
            ICasesAppService casesAppService,
            IProcedureTypesAppService procedureTypesAppService,
            IRepository<Facility, int> facilityRepository,
            IRepository<Personnel, int> personnelRepository)
        {
            _casesAppService = casesAppService;
            _procedureTypesAppService = procedureTypesAppService;
            _facilityRepository = facilityRepository;
            _personnelRepository = personnelRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateOrEditModal(int? id)
        {
            CreateOrEditCaseModalViewModel viewModel;

            if (id.HasValue)
            {
                var output = await _casesAppService.GetCaseForEdit(new EntityDto<int> { Id = id.Value });
                viewModel = new CreateOrEditCaseModalViewModel
                {
                    Case = output.Case,
                    IsEditMode = true
                };
            }
            else
            {
                viewModel = new CreateOrEditCaseModalViewModel
                {
                    Case = new CreateOrEditCaseDto
                    {
                        CaseDate = DateTime.Now,
                        Status = "Open",
                        TotalAmount = 0
                    },
                    IsEditMode = false
                };
            }

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var output = await _casesAppService.GetCaseForView(id);
            return View(output.Case);
        }

        public async Task<IActionResult> AddOrEditProductModal(int caseId, int? productId = null)
        {
            CaseProductDto model;
            if (productId.HasValue && productId.Value > 0)
            {
                model = await _casesAppService.GetCaseProductForEdit(productId.Value);
            }
            else
            {
                model = new CaseProductDto { CaseId = caseId, Quantity = 1, Discount = 0 };
            }
            return PartialView("_AddOrEditProductModal", model);
        }

        [HttpGet]
        public async Task<JsonResult> GetProcedureTypes()
        {
            var procedureTypes = await _procedureTypesAppService.GetAllActiveProcedureTypes();
            return Json(procedureTypes.Select(pt => new { id = pt.Id, name = pt.Name }));
        }

        [HttpGet]
        public async Task<JsonResult> GetFacilities()
        {
            var facilities = await _facilityRepository.GetAll()
                .OrderBy(f => f.FacilityName)
                .Select(f => new { id = f.Id, name = f.FacilityName })
                .ToListAsync();
            return Json(facilities);
        }

        [HttpGet]
        public async Task<JsonResult> GetPersonnel()
        {
            var raw = await _personnelRepository.GetAll()
                .OrderBy(p => p.LAST_NAME).ThenBy(p => p.FIRST_NAME)
                .Select(p => new { p.Id, p.FIRST_NAME, p.LAST_NAME })
                .ToListAsync();

            var result = raw.Select(p => new
            {
                id = p.Id,
                name = $"{p.FIRST_NAME} {p.LAST_NAME}".Trim()
            });

            return Json(result);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using ATI.Revenue.Application.Cases;
using ATI.Revenue.Application.Cases.Dtos;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using ATI.Web.Controllers;
using ATI.Revenue.Web.PageModel.Cases;
using System;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    public class CasesController : ATIControllerBase
    {
        private readonly ICasesAppService _casesAppService;

        public CasesController(ICasesAppService casesAppService)
        {
            _casesAppService = casesAppService;
        }

        // View for listing cases
        public IActionResult Index()
        {
            return View();
        }

        // Modal view for creating/editing cases
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

        // View for case details
        public async Task<IActionResult> Details(int id)
        {
            var output = await _casesAppService.GetCaseForView(id);
            return View(output.Case);
        }

        // Modal for adding/editing case products
        public IActionResult AddOrEditProductModal(int caseId, int? productId = null)
        {
            var model = new CaseProductDto
            {
                CaseId = caseId,
                Quantity = 1,
                Discount = 0
            };

            // If editing, you could load the existing product data here
            // For now, we'll handle that on the client side

            return PartialView("_AddOrEditProductModal", model);
        }
    }
}
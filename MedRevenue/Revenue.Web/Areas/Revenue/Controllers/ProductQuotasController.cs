using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATI.Revenue.Application.ProductQuotas;
using ATI.Revenue.Application.ProductQuotas.Dtos;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using ATI.Web.Controllers;
using ATI.Revenue.Web.PageModel.ProductQuotas;
using ATI.Admin.Domain.Entities;
using ATI.Revenue.Domain.Entities;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    public class ProductQuotasController : ATIControllerBase
    {
        private readonly IProductQuotasAppService _productQuotasAppService;
        private readonly IRepository<Facility, int> _facilityRepository;
        private readonly IRepository<ProductCategory, int> _productCategoryRepository;

        public ProductQuotasController(
            IProductQuotasAppService productQuotasAppService,
            IRepository<Facility, int> facilityRepository,
            IRepository<ProductCategory, int> productCategoryRepository)
        {
            _productQuotasAppService = productQuotasAppService;
            _facilityRepository = facilityRepository;
            _productCategoryRepository = productCategoryRepository;
        }

        // View for listing product quotas
        public async Task<IActionResult> Index()
        {
            // Populate filter dropdowns
            ViewBag.Hospitals = await GetHospitalSelectList();
            ViewBag.ProductCategories = await GetProductCategorySelectList();
            ViewBag.CurrentYear = DateTime.Now.Year;
            ViewBag.CurrentMonth = DateTime.Now.Month;

            return View();
        }

        // Modal view for creating/editing product quotas
        public async Task<IActionResult> CreateOrEditModal(int? id)
        {
            CreateOrEditProductQuotaModalViewModel viewModel;

            if (id.HasValue)
            {
                var output = await _productQuotasAppService.GetProductQuotaForEdit(new EntityDto<int> { Id = id.Value });
                viewModel = new CreateOrEditProductQuotaModalViewModel
                {
                    ProductQuota = output.ProductQuota,
                    IsEditMode = true
                };
            }
            else
            {
                viewModel = new CreateOrEditProductQuotaModalViewModel
                {
                    ProductQuota = new CreateOrEditProductQuotaDto
                    {
                        PeriodYear = DateTime.Now.Year,
                        PeriodMonth = DateTime.Now.Month,
                        TargetAmount = 0
                    },
                    IsEditMode = false
                };
            }

            // Populate dropdowns
            ViewBag.Hospitals = await GetHospitalSelectList();
            ViewBag.ProductCategories = await GetProductCategorySelectList();

            return PartialView("_CreateOrEditModal", viewModel);
        }

        // View for product quota details
        public async Task<IActionResult> Details(int id)
        {
            var output = await _productQuotasAppService.GetProductQuotaForView(id);
            return View(output.ProductQuota);
        }

        // Helper methods for dropdowns
        private async Task<SelectList> GetHospitalSelectList()
        {
            var hospitals = await _facilityRepository.GetAllListAsync();
            return new SelectList(
                hospitals.OrderBy(h => h.FacilityName),
                "Id",
                "FacilityName"
            );
        }

        private async Task<SelectList> GetProductCategorySelectList()
        {
            var categories = await _productCategoryRepository.GetAll()
                .Select(c => new { c.Id, c.Name })
                .OrderBy(c => c.Name)
                .ToListAsync();
            return new SelectList(categories, "Id", "Name");
        }
    }
}

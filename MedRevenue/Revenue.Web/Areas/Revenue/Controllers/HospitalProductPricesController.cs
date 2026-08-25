using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using ATI.Admin.Domain.Entities;
using ATI.Revenue.Application.HospitalProductPrices;
using ATI.Revenue.Application.HospitalProductPrices.Dtos;
using ATI.Revenue.Domain.Entities;
using ATI.Revenue.Web.PageModel.HospitalProductPrices;
using ATI.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using ATI.Authorization;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    [AbpMvcAuthorize(AppPermissions.Pages_Revenue_HospitalProductPrices)]
    public class HospitalProductPricesController : ATIControllerBase
    {
        private readonly IHospitalProductPricesAppService _hospitalProductPricesAppService;
        private readonly IRepository<Facility, int> _facilityRepository;
        private readonly IRepository<Product, int> _productRepository;

        public HospitalProductPricesController(
            IHospitalProductPricesAppService hospitalProductPricesAppService,
            IRepository<Facility, int> facilityRepository,
            IRepository<Product, int> productRepository)
        {
            _hospitalProductPricesAppService = hospitalProductPricesAppService;
            _facilityRepository = facilityRepository;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Hospitals = await GetHospitalSelectList();
            return View();
        }

        public async Task<IActionResult> CreateOrEditModal(int? id)
        {
            CreateOrEditHospitalProductPriceModalViewModel viewModel;

            if (id.HasValue)
            {
                var output = await _hospitalProductPricesAppService.GetHospitalProductPriceForEdit(new EntityDto<int> { Id = id.Value });
                viewModel = new CreateOrEditHospitalProductPriceModalViewModel
                {
                    HospitalProductPrice = output.HospitalProductPrice,
                    IsEditMode = true
                };
            }
            else
            {
                viewModel = new CreateOrEditHospitalProductPriceModalViewModel
                {
                    HospitalProductPrice = new CreateOrEditHospitalProductPriceDto
                    {
                        EffectiveDate = DateTime.Today,
                        IsActive = true
                    },
                    IsEditMode = false
                };
            }

            ViewBag.Hospitals = await GetHospitalSelectList();
            ViewBag.Products = await GetProductSelectList();

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var output = await _hospitalProductPricesAppService.GetHospitalProductPriceForView(id);
            return View(output.HospitalProductPrice);
        }

        private async Task<SelectList> GetHospitalSelectList()
        {
            var hospitals = await _facilityRepository.GetAll()
                .Select(h => new { h.Id, FacilityName = h.FacilityName ?? "" })
                .OrderBy(h => h.FacilityName)
                .ToListAsync();
            return new SelectList(hospitals, "Id", "FacilityName");
        }

        private async Task<SelectList> GetProductSelectList()
        {
            var products = await _productRepository.GetAll()
                .Select(p => new { p.Id, Name = p.Name ?? "", ProductCode = p.ProductCode ?? "" })
                .OrderBy(p => p.Name)
                .ToListAsync();
            return new SelectList(
                products.Select(p => new { p.Id, Display = string.IsNullOrEmpty(p.ProductCode) ? p.Name : $"{p.ProductCode} - {p.Name}" }),
                "Id",
                "Display"
            );
        }
    }
}

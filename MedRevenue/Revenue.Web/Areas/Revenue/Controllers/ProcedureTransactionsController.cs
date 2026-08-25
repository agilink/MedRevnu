using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATI.Revenue.Application.ProcedureTransactions;
using ATI.Revenue.Application.ProcedureTransactions.Dtos;
using ATI.Revenue.Application.Products;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using ATI.Revenue.Application.Products.Dtos;
using System.Linq;
using System.Threading.Tasks;
using ATI.Web.Controllers;
using ATI.Revenue.Domain.Enums;
using ATI.Revenue.Web.PageModel.ProcedureTransactions;
using ATI.Admin.Domain.Entities;
using Abp.AspNetCore.Mvc.Authorization;
using ATI.Authorization;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    [AbpMvcAuthorize(AppPermissions.Pages_Revenue_ProcedureTransactions)]
    public class ProcedureTransactionsController : ATIControllerBase
    {
        private readonly IProcedureTransactionsAppService _procedureTransactionsAppService;
        private readonly IRepository<Personnel, int> _personnelRepository;
        private readonly IRepository<Facility, int> _facilityRepository;
        private readonly IProductsAppService _productsAppService;

        public ProcedureTransactionsController(
            IProcedureTransactionsAppService procedureTransactionsAppService,
            IRepository<Personnel, int> personnelRepository,
            IRepository<Facility, int> facilityRepository,
            IProductsAppService productsAppService)
        {
            _procedureTransactionsAppService = procedureTransactionsAppService;
            _personnelRepository = personnelRepository;
            _facilityRepository = facilityRepository;
            _productsAppService = productsAppService;
        }

        // View for listing procedure transactions
        public async Task<IActionResult> Index()
        {
            // Populate filter dropdowns
            ViewBag.Hospitals = await GetHospitalSelectList();
            ViewBag.Physicians = await GetPhysicianSelectList();
            ViewBag.CurrentYear = DateTime.Now.Year;
            ViewBag.CurrentMonth = DateTime.Now.Month;

            return View();
        }

        // Modal view for creating/editing procedure transactions
        public async Task<IActionResult> CreateOrEditModal(int? id)
        {
            CreateOrEditProcedureTransactionModalViewModel viewModel;

            if (id.HasValue)
            {
                var output = await _procedureTransactionsAppService.GetProcedureTransactionForEdit(new EntityDto<int> { Id = id.Value });
                viewModel = new CreateOrEditProcedureTransactionModalViewModel
                {
                    ProcedureTransaction = output.ProcedureTransaction,
                    IsEditMode = true
                };
            }
            else
            {
                viewModel = new CreateOrEditProcedureTransactionModalViewModel
                {
                    ProcedureTransaction = new CreateOrEditProcedureTransactionDto
                    {
                        ProcedureDate = DateTime.Now,
                        ImplantType = ImplantType.DeNovo,
                        Quantity = 1,
                        UnitPrice = 0,
                        TotalAmount = 0
                    },
                    IsEditMode = false
                };
            }

            // Populate dropdowns
            ViewBag.Hospitals = await GetHospitalSelectList();
            ViewBag.Physicians = await GetPhysicianSelectList();
            ViewBag.Products = await GetProductSelectList();

            return PartialView("_CreateOrEditModal", viewModel);
        }

        // View for procedure transaction details
        public async Task<IActionResult> Details(int id)
        {
            var output = await _procedureTransactionsAppService.GetProcedureTransactionForView(id);
            return View(output.ProcedureTransaction);
        }

        // API endpoint to get physician's facility
        [HttpPost]
        public async Task<JsonResult> GetPhysicianFacility(int? physicianId)
        {
            // Clearing the physician is a legitimate state, not an error.
            if (!physicianId.HasValue)
            {
                return Json(new { success = true, facilityId = (int?)null, facilityName = "" });
            }

            try
            {
                var physician = await _personnelRepository.GetAsync(physicianId.Value);
                var facility = physician.FacilityId.HasValue
                    ? await _facilityRepository.GetAsync(physician.FacilityId.Value)
                    : null;

                return Json(new {
                    success = true,
                    facilityId = physician.FacilityId,
                    facilityName = facility?.FacilityName ?? ""
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API endpoint to get product price based on hospital and product
        [HttpPost]
        public async Task<JsonResult> GetProductPriceByHospital(int? hospitalId, int? productId)
        {
            // Both are needed to resolve a contracted price; without them there is
            // simply no price to offer yet.
            if (!hospitalId.HasValue || !productId.HasValue)
            {
                return Json(new { success = true, unitPrice = (decimal?)null });
            }

            try
            {
                var price = await _procedureTransactionsAppService.GetProductPriceByHospital(hospitalId.Value, productId.Value);
                return Json(new { success = true, unitPrice = price });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Helper methods for dropdowns
        private async Task<SelectList> GetHospitalSelectList()
        {
            var hospitals = await _facilityRepository.GetAll()
                .Select(h => new { h.Id, FacilityName = h.FacilityName ?? "" })
                .OrderBy(h => h.FacilityName)
                .ToListAsync();
            return new SelectList(hospitals, "Id", "FacilityName");
        }

        private async Task<SelectList> GetPhysicianSelectList()
        {
            var physicianList = await _personnelRepository.GetAll()
                .Where(p => p.FIRST_NAME != null || p.LAST_NAME != null)
                .Select(p => new
                {
                    p.Id,
                    FirstName = p.FIRST_NAME ?? "",
                    LastName = p.LAST_NAME ?? ""
                })
                .ToListAsync();

            var namedList = physicianList
                .Select(p => new { p.Id, Name = (p.FirstName + " " + p.LastName).Trim() })
                .OrderBy(p => p.Name)
                .ToList();

            return new SelectList(namedList, "Id", "Name");
        }

        /// <summary>
        /// Active products ordered by name. Returns the DTOs rather than a SelectList so
        /// the view can emit each product's implant type as a data attribute, letting the
        /// form set the De Novo / Gen Change radio from the product chosen.
        /// </summary>
        private async Task<List<ProductDto>> GetProductSelectList()
        {
            var productsResult = await _productsAppService.GetAllActive();
            return productsResult.Items.OrderBy(p => p.Name).ToList();
        }
    }
}

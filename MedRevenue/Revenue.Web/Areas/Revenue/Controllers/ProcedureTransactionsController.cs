using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATI.Revenue.Application.ProcedureTransactions;
using ATI.Revenue.Application.ProcedureTransactions.Dtos;
using ATI.Revenue.Application.Products;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using ATI.Web.Controllers;
using ATI.Revenue.Web.PageModel.ProcedureTransactions;
using ATI.Admin.Domain.Entities;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
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
                        ProcedureType = "DE_NOVO", // Default to NEW
                        Quantity = 1,
                        UnitPrice = 0,
                        TotalAmount = 0
                    },
                    IsEditMode = false
                };
            }

            // Populate dropdowns
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

        // API endpoint to get product base price based on procedure type
        [HttpPost]
        public async Task<JsonResult> GetProductBasePrice(int productId, string procedureType)
        {
            try
            {
                var basePrice = await _procedureTransactionsAppService.GetProductBasePrice(productId, procedureType);
                return Json(new { success = true, basePrice = basePrice });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API endpoint to get physician's facility
        [HttpPost]
        public async Task<JsonResult> GetPhysicianFacility(int physicianId)
        {
            try
            {
                var physician = await _personnelRepository.GetAsync(physicianId);
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

        private async Task<SelectList> GetProductSelectList()
        {
            var productsResult = await _productsAppService.GetAllActive();
            return new SelectList(
                productsResult.Items.OrderBy(p => p.Name),
                "Id",
                "Name"
            );
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATI.Revenue.Application.Authorization;
using ATI.Revenue.Application.ProcedureTransactions;
using ATI.Revenue.Application.ProcedureTransactions.Dtos;
using ATI.Revenue.Application.Products;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
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
        private readonly IPhysicianDataScopeProvider _scopeProvider;

        public ProcedureTransactionsController(
            IProcedureTransactionsAppService procedureTransactionsAppService,
            IRepository<Personnel, int> personnelRepository,
            IRepository<Facility, int> facilityRepository,
            IProductsAppService productsAppService,
            IPhysicianDataScopeProvider scopeProvider)
        {
            _procedureTransactionsAppService = procedureTransactionsAppService;
            _personnelRepository = personnelRepository;
            _facilityRepository = facilityRepository;
            _productsAppService = productsAppService;
            _scopeProvider = scopeProvider;
        }

        // View for listing procedure transactions
        public async Task<IActionResult> Index()
        {
            // Filter dropdowns, narrowed to what this user is allowed to ask about. The
            // list itself is already scoped server-side; offering other hospitals and
            // physicians here would only advertise names they cannot reach.
            var scope = await _scopeProvider.GetAsync();

            ViewBag.Hospitals = await GetHospitalSelectList(scope.IsRestricted ? scope.HospitalId : null);
            ViewBag.Physicians = await GetPhysicianSelectList(
                scope.IsRestricted ? scope.PhysicianId : null,
                scope.IsRestricted ? scope.HospitalId : null);
            ViewBag.IsPhysicianRestricted = scope.IsRestricted;
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
                        TotalAmount = 0
                    },
                    IsEditMode = false
                };
            }

            // A physician gets one hospital and themselves, both preselected, so the case
            // can only be recorded against what they are actually allowed to record.
            var scope = await _scopeProvider.GetAsync();

            ViewBag.Hospitals = await GetHospitalSelectList(scope.IsRestricted ? scope.HospitalId : null);
            ViewBag.Physicians = await GetPhysicianSelectList(
                scope.IsRestricted ? scope.PhysicianId : null,
                scope.IsRestricted ? scope.HospitalId : null);
            ViewBag.IsPhysicianRestricted = scope.IsRestricted;
            ViewBag.Products = await GetProductSelectList();

            return PartialView("_CreateOrEditModal", viewModel);
        }

        // View for procedure transaction details
        public async Task<IActionResult> Details(int id)
        {
            var output = await _procedureTransactionsAppService.GetProcedureTransactionForView(id);
            return View(output.ProcedureTransaction);
        }

        /// <summary>
        /// Physicians at a hospital, for cascading the two dropdowns on the case form.
        /// </summary>
        /// <remarks>
        /// Passing no hospital returns every physician, so clearing the hospital widens the
        /// list again rather than emptying it.
        /// </remarks>
        [HttpGet]
        public async Task<JsonResult> GetPhysiciansByHospital(int? hospitalId)
        {
            // A restricted user's own scope wins over whatever the page asked for,
            // otherwise this endpoint would happily list any hospital's physicians to
            // anyone who called it with a different id.
            var scope = await _scopeProvider.GetAsync();

            if (scope.IsRestricted)
            {
                hospitalId = scope.HospitalId;
            }

            var physicians = await _personnelRepository.GetAll()
                .WhereIf(scope.SeesNothing, p => false)
                .WhereIf(scope.IsRestricted && scope.PhysicianId.HasValue, p => p.Id == scope.PhysicianId.Value)
                .WhereIf(hospitalId.HasValue, p => p.FacilityId == hospitalId.Value)
                .OrderBy(p => p.LAST_NAME).ThenBy(p => p.FIRST_NAME)
                .Select(p => new
                {
                    id = p.Id,
                    name = ((p.FIRST_NAME ?? "") + " " + (p.LAST_NAME ?? "")).Trim(),
                    hospitalId = p.FacilityId
                })
                .ToListAsync();

            return Json(new { success = true, physicians });
        }

        // API endpoint to get physician's facility
        [HttpGet]
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
        /// <summary>
        /// Every hospital, or just the one a restricted user is confined to. The single
        /// entry is preselected, so a physician never has to choose it.
        /// </summary>
        private async Task<SelectList> GetHospitalSelectList(int? onlyHospitalId)
        {
            var hospitals = await _facilityRepository.GetAll()
                .WhereIf(onlyHospitalId.HasValue, h => h.Id == onlyHospitalId.Value)
                .Select(h => new { h.Id, FacilityName = h.FacilityName ?? "" })
                .OrderBy(h => h.FacilityName)
                .ToListAsync();

            return new SelectList(hospitals, "Id", "FacilityName", onlyHospitalId);
        }

        /// <summary>
        /// The whole roster, or just the signed-in physician. Narrowing this is not only
        /// convenience: the roster carries the names of every physician at every hospital,
        /// which is not a physician user's to browse.
        /// </summary>
        private async Task<SelectList> GetPhysicianSelectList(int? onlyPhysicianId, int? onlyHospitalId)
        {
            var physicianList = await _personnelRepository.GetAll()
                .Where(p => p.FIRST_NAME != null || p.LAST_NAME != null)
                .WhereIf(onlyPhysicianId.HasValue, p => p.Id == onlyPhysicianId.Value)
                // A restricted user with no personnel record of their own still must not
                // see other hospitals' physicians.
                .WhereIf(!onlyPhysicianId.HasValue && onlyHospitalId.HasValue,
                    p => p.FacilityId == onlyHospitalId.Value)
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

            return new SelectList(namedList, "Id", "Name", onlyPhysicianId);
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

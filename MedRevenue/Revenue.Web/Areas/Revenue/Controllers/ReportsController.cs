using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ATI.Revenue.Application.Reports;
using ATI.Revenue.Application.Reports.Dtos;
using Abp.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using ATI.Web.Controllers;
using ATI.Admin.Domain.Entities;
using ATI.Revenue.Domain.Entities;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    public class ReportsController : ATIControllerBase
    {
        private readonly IReportsAppService _reportsAppService;
        private readonly IRepository<Facility, int> _facilityRepository;
        private readonly IRepository<ProductCategory, int> _productCategoryRepository;
        private readonly IRepository<Personnel, int> _personnelRepository;

        public ReportsController(
            IReportsAppService reportsAppService,
            IRepository<Facility, int> facilityRepository,
            IRepository<ProductCategory, int> productCategoryRepository,
            IRepository<Personnel, int> personnelRepository)
        {
            _reportsAppService = reportsAppService;
            _facilityRepository = facilityRepository;
            _productCategoryRepository = productCategoryRepository;
            _personnelRepository = personnelRepository;
        }

        // Report 1: Rate Chart - Products by ProductCategory with prices for a hospital
        public async Task<IActionResult> RateChart()
        {
            ViewBag.Hospitals = await GetHospitalSelectList();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetRateChartData(int hospitalId)
        {
            var data = await _reportsAppService.GetRateChartReport(new RateChartReportInput
            {
                HospitalId = hospitalId
            });

            return Json(new { success = true, data = data });
        }

        // Report 2: Monthly Revenue - Daily revenue by ProductCategory for each month
        public async Task<IActionResult> MonthlyRevenue()
        {
            ViewBag.Hospitals = await GetHospitalSelectList();
            ViewBag.CurrentYear = DateTime.Now.Year;
            ViewBag.CurrentMonth = DateTime.Now.Month;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetMonthlyRevenueData(int year, int month, int? hospitalId)
        {
            var data = await _reportsAppService.GetMonthlyRevenueReport(new MonthlyRevenueReportInput
            {
                Year = year,
                Month = month,
                HospitalId = hospitalId
            });

            return Json(new { success = true, data = data });
        }

        // Report 3: Cases by Person - Cases per physician for last year
        public async Task<IActionResult> CasesByPerson()
        {
            ViewBag.Hospitals = await GetHospitalSelectList();
            ViewBag.Physicians = await GetPhysicianSelectList();
            ViewBag.LastYear = DateTime.Now.Year - 1;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetCasesByPersonData(int year, int? hospitalId, int? physicianId)
        {
            var data = await _reportsAppService.GetCasesByPersonReport(new CasesByPersonReportInput
            {
                Year = year,
                HospitalId = hospitalId,
                PhysicianId = physicianId
            });

            return Json(new { success = true, data = data });
        }

        // Report 4: Transaction Amount - By physician per product type for last year
        public async Task<IActionResult> TransactionAmount()
        {
            ViewBag.Physicians = await GetPhysicianSelectList();
            ViewBag.ProductCategories = await GetProductCategorySelectList();
            ViewBag.LastYear = DateTime.Now.Year - 1;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetTransactionAmountData(int year, int? physicianId, int? productCategoryId)
        {
            var data = await _reportsAppService.GetTransactionAmountReport(new TransactionAmountReportInput
            {
                Year = year,
                PhysicianId = physicianId,
                ProductCategoryId = productCategoryId
            });

            return Json(new { success = true, data = data });
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

        private async Task<SelectList> GetPhysicianSelectList()
        {
            var physicians = await _personnelRepository.GetAllListAsync();
            var physicianList = physicians
                .Where(p => !string.IsNullOrEmpty(p.FIRST_NAME) || !string.IsNullOrEmpty(p.LAST_NAME))
                .Select(p => new
                {
                    Id = p.Id,
                    Name = $"{p.FIRST_NAME} {p.LAST_NAME}".Trim()
                })
                .OrderBy(p => p.Name)
                .ToList();

            return new SelectList(physicianList, "Id", "Name");
        }

        private async Task<SelectList> GetProductCategorySelectList()
        {
            var categories = await _productCategoryRepository.GetAllListAsync();
            return new SelectList(
                categories.OrderBy(c => c.Name),
                "Id",
                "Name"
            );
        }
    }
}

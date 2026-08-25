using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ATI.Revenue.Application.Reports;
using ATI.Revenue.Application.Reports.Dtos;
using ATI.Revenue.Application.Reports.Exporting;
using Abp.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using ATI.Web.Controllers;
using ATI.Admin.Domain.Entities;
using ATI.Revenue.Domain.Entities;
using Abp.AspNetCore.Mvc.Authorization;
using ATI.Authorization;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    [AbpMvcAuthorize(AppPermissions.Pages_Revenue_Reports)]
    public class ReportsController : ATIControllerBase
    {
        private readonly IReportsAppService _reportsAppService;
        private readonly IRepository<Facility, int> _facilityRepository;
        private readonly IRepository<ProductCategory, int> _productCategoryRepository;
        private readonly IRepository<Personnel, int> _personnelRepository;
        private readonly IRevenueReportsExcelExporter _excelExporter;

        public ReportsController(
            IReportsAppService reportsAppService,
            IRepository<Facility, int> facilityRepository,
            IRepository<ProductCategory, int> productCategoryRepository,
            IRepository<Personnel, int> personnelRepository,
            IRevenueReportsExcelExporter excelExporter)
        {
            _reportsAppService = reportsAppService;
            _facilityRepository = facilityRepository;
            _productCategoryRepository = productCategoryRepository;
            _personnelRepository = personnelRepository;
            _excelExporter = excelExporter;
        }

        // Report 1: Rate Chart - Products by ProductCategory with prices for a hospital
        public async Task<IActionResult> RateChart()
        {
            ViewBag.Hospitals = await GetHospitalSelectList();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetRateChartData(int? hospitalId)
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
        public async Task<JsonResult> GetMonthlyRevenueData(int? year, int? month, int? hospitalId)
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
        public async Task<JsonResult> GetCasesByPersonData(int? year, int? hospitalId, int? physicianId)
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
        public async Task<JsonResult> GetTransactionAmountData(int? year, int? physicianId, int? productCategoryId)
        {
            var data = await _reportsAppService.GetTransactionAmountReport(new TransactionAmountReportInput
            {
                Year = year,
                PhysicianId = physicianId,
                ProductCategoryId = productCategoryId
            });

            return Json(new { success = true, data = data });
        }

        // Report 5: Quarterly Rollup - three months of targets against actuals
        public async Task<IActionResult> QuarterlyRollup()
        {
            ViewBag.Hospitals = await GetHospitalSelectList();
            ViewBag.CurrentYear = DateTime.Now.Year;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetQuarterlyRollupData(int? year, int? quarter, int? hospitalId)
        {
            var data = await _reportsAppService.GetQuarterlyRollupReport(new QuarterlyRollupReportInput
            {
                Year = year,
                Quarter = quarter,
                HospitalId = hospitalId
            });

            return Json(new { success = true, data = data });
        }

        // Excel export for each report. The client keeps this data in a workbook today,
        // so being able to take a report back out to Excel is what lets them stop
        // maintaining it by hand while still sharing numbers the way they already do.

        [HttpPost]
        public async Task<JsonResult> ExportRateChart(int? hospitalId)
        {
            var data = await _reportsAppService.GetRateChartReport(new RateChartReportInput { HospitalId = hospitalId });
            return Json(_excelExporter.ExportRateChart(data));
        }

        [HttpPost]
        public async Task<JsonResult> ExportMonthlyRevenue(int? year, int? month, int? hospitalId)
        {
            var data = await _reportsAppService.GetMonthlyRevenueReport(new MonthlyRevenueReportInput
            {
                Year = year,
                Month = month,
                HospitalId = hospitalId
            });
            return Json(_excelExporter.ExportMonthlyRevenue(data));
        }

        [HttpPost]
        public async Task<JsonResult> ExportCasesByPerson(int? year, int? hospitalId, int? physicianId)
        {
            var data = await _reportsAppService.GetCasesByPersonReport(new CasesByPersonReportInput
            {
                Year = year,
                HospitalId = hospitalId,
                PhysicianId = physicianId
            });
            return Json(_excelExporter.ExportCasesByPerson(data));
        }

        [HttpPost]
        public async Task<JsonResult> ExportTransactionAmount(int? year, int? physicianId, int? productCategoryId)
        {
            var data = await _reportsAppService.GetTransactionAmountReport(new TransactionAmountReportInput
            {
                Year = year,
                PhysicianId = physicianId,
                ProductCategoryId = productCategoryId
            });
            return Json(_excelExporter.ExportTransactionAmount(data));
        }

        [HttpPost]
        public async Task<JsonResult> ExportQuarterlyRollup(int? year, int? quarter, int? hospitalId)
        {
            var data = await _reportsAppService.GetQuarterlyRollupReport(new QuarterlyRollupReportInput
            {
                Year = year,
                Quarter = quarter,
                HospitalId = hospitalId
            });
            return Json(_excelExporter.ExportQuarterlyRollup(data));
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

        private async Task<SelectList> GetProductCategorySelectList()
        {
            var categories = await _productCategoryRepository.GetAll()
                .Select(c => new { c.Id, Name = c.Name ?? "" })
                .OrderBy(c => c.Name)
                .ToListAsync();
            return new SelectList(categories, "Id", "Name");
        }
    }
}

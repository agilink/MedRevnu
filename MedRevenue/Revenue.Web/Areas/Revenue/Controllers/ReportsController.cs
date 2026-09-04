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
        private readonly IRepository<ProcedureTransaction, int> _procedureTransactionRepository;
        private readonly IRepository<Product, int> _productRepository;

        public ReportsController(
            IReportsAppService reportsAppService,
            IRepository<Facility, int> facilityRepository,
            IRepository<ProductCategory, int> productCategoryRepository,
            IRepository<Personnel, int> personnelRepository,
            IRevenueReportsExcelExporter excelExporter,
            IRepository<ProcedureTransaction, int> procedureTransactionRepository,
            IRepository<Product, int> productRepository)
        {
            _reportsAppService = reportsAppService;
            _facilityRepository = facilityRepository;
            _productCategoryRepository = productCategoryRepository;
            _personnelRepository = personnelRepository;
            _excelExporter = excelExporter;
            _procedureTransactionRepository = procedureTransactionRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        /// The month the monthly report should open on.
        /// </summary>
        /// <remarks>
        /// The latest month that actually has a case, not the current one. This report
        /// covers a single month, so defaulting to today meant it opened empty for the
        /// whole of any month before the first case was entered - which is exactly what
        /// happened: the cases sit in April and August and the report opened on September
        /// and found nothing.
        ///
        /// Falls back to today on an empty database, where there is no better answer.
        /// </remarks>
        private async Task<DateTime> GetLatestMonthWithData()
        {
            var latest = await _procedureTransactionRepository.GetAll()
                .OrderByDescending(pt => pt.ProcedureDate)
                .Select(pt => (DateTime?)pt.ProcedureDate)
                .FirstOrDefaultAsync();

            return latest ?? DateTime.Now;
        }

        // Report 1: Rate Chart - Products by ProductCategory with prices for a hospital
        public async Task<IActionResult> RateChart()
        {
            ViewBag.Hospitals = await GetHospitalSelectList();
            ViewBag.ProductCodes = await GetProductCodeSelectList();
            return View();
        }

        /// <summary>
        /// Distinct product codes actually in use, for the rate chart's code filter.
        /// </summary>
        private async Task<SelectList> GetProductCodeSelectList()
        {
            var codes = await _productRepository.GetAll()
                .Where(p => p.IsActive && p.ProductCode != null && p.ProductCode != "")
                .Select(p => p.ProductCode)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return new SelectList(codes.Select(c => new { Value = c, Text = c }), "Value", "Text");
        }

        [HttpPost]
        public async Task<JsonResult> GetRateChartData([FromBody] RateChartReportInput input)
        {
            // [FromBody] and a single input, not loose parameters. The page posts a
            // JSON body and simple types never bind from one, so every filter on this
            // report used to arrive null and the report ran on its defaults.
            var data = await _reportsAppService.GetRateChartReport(input ?? new RateChartReportInput());

            return Json(new { success = true, data = data });
        }

        // Report 2: Monthly Revenue - Daily revenue by ProductCategory for each month
        public async Task<IActionResult> MonthlyRevenue()
        {
            var openOn = await GetLatestMonthWithData();

            ViewBag.Hospitals = await GetHospitalSelectList();
            ViewBag.CurrentYear = openOn.Year;
            ViewBag.CurrentMonth = openOn.Month;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetMonthlyRevenueData([FromBody] MonthlyRevenueReportInput input)
        {
            // [FromBody] and a single input, not loose parameters. The page posts a
            // JSON body and simple types never bind from one, so every filter on this
            // report used to arrive null and the report ran on its defaults.
            var data = await _reportsAppService.GetMonthlyRevenueReport(input ?? new MonthlyRevenueReportInput());

            return Json(new { success = true, data = data });
        }

        // Report 3: Cases by Person - Cases per physician for the selected year
        public async Task<IActionResult> CasesByPerson()
        {
            ViewBag.Hospitals = await GetHospitalSelectList();
            ViewBag.Physicians = await GetPhysicianSelectList();
            ViewBag.CurrentYear = DateTime.Now.Year;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetCasesByPersonData([FromBody] CasesByPersonReportInput input)
        {
            // [FromBody] and a single input, not loose parameters. The page posts a
            // JSON body and simple types never bind from one, so every filter on this
            // report used to arrive null and the report ran on its defaults.
            var data = await _reportsAppService.GetCasesByPersonReport(input ?? new CasesByPersonReportInput());

            return Json(new { success = true, data = data });
        }

        // Report 4: Transaction Amount - By physician per product type for the selected year
        public async Task<IActionResult> TransactionAmount()
        {
            ViewBag.Physicians = await GetPhysicianSelectList();
            ViewBag.ProductCategories = await GetProductCategorySelectList();
            ViewBag.CurrentYear = DateTime.Now.Year;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetTransactionAmountData([FromBody] TransactionAmountReportInput input)
        {
            // [FromBody] and a single input, not loose parameters. The page posts a
            // JSON body and simple types never bind from one, so every filter on this
            // report used to arrive null and the report ran on its defaults.
            var data = await _reportsAppService.GetTransactionAmountReport(input ?? new TransactionAmountReportInput());

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
        public async Task<JsonResult> GetQuarterlyRollupData([FromBody] QuarterlyRollupReportInput input)
        {
            // [FromBody] and a single input, not loose parameters. The page posts a
            // JSON body and simple types never bind from one, so every filter on this
            // report used to arrive null and the report ran on its defaults.
            var data = await _reportsAppService.GetQuarterlyRollupReport(input ?? new QuarterlyRollupReportInput());

            return Json(new { success = true, data = data });
        }

        // Excel export for each report. The client keeps this data in a workbook today,
        // so being able to take a report back out to Excel is what lets them stop
        // maintaining it by hand while still sharing numbers the way they already do.

        [HttpPost]
        public async Task<JsonResult> ExportRateChart([FromBody] RateChartReportInput input)
        {
            // [FromBody] and a single input, not loose parameters. The page posts a
            // JSON body and simple types never bind from one, so every filter on this
            // report used to arrive null and the report ran on its defaults.
            var data = await _reportsAppService.GetRateChartReport(input ?? new RateChartReportInput());

            return Json(_excelExporter.ExportRateChart(data));
        }

        [HttpPost]
        public async Task<JsonResult> ExportMonthlyRevenue([FromBody] MonthlyRevenueReportInput input)
        {
            // [FromBody] and a single input, not loose parameters. The page posts a
            // JSON body and simple types never bind from one, so every filter on this
            // report used to arrive null and the report ran on its defaults.
            var data = await _reportsAppService.GetMonthlyRevenueReport(input ?? new MonthlyRevenueReportInput());

            return Json(_excelExporter.ExportMonthlyRevenue(data));
        }

        [HttpPost]
        public async Task<JsonResult> ExportCasesByPerson([FromBody] CasesByPersonReportInput input)
        {
            // [FromBody] and a single input, not loose parameters. The page posts a
            // JSON body and simple types never bind from one, so every filter on this
            // report used to arrive null and the report ran on its defaults.
            var data = await _reportsAppService.GetCasesByPersonReport(input ?? new CasesByPersonReportInput());

            return Json(_excelExporter.ExportCasesByPerson(data));
        }

        [HttpPost]
        public async Task<JsonResult> ExportTransactionAmount([FromBody] TransactionAmountReportInput input)
        {
            // [FromBody] and a single input, not loose parameters. The page posts a
            // JSON body and simple types never bind from one, so every filter on this
            // report used to arrive null and the report ran on its defaults.
            var data = await _reportsAppService.GetTransactionAmountReport(input ?? new TransactionAmountReportInput());

            return Json(_excelExporter.ExportTransactionAmount(data));
        }

        [HttpPost]
        public async Task<JsonResult> ExportQuarterlyRollup([FromBody] QuarterlyRollupReportInput input)
        {
            // [FromBody] and a single input, not loose parameters. The page posts a
            // JSON body and simple types never bind from one, so every filter on this
            // report used to arrive null and the report ran on its defaults.
            var data = await _reportsAppService.GetQuarterlyRollupReport(input ?? new QuarterlyRollupReportInput());

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

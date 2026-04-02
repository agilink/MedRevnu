using ATI.Revenue.Application.Dashboard;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    public class DashboardController : Controller
    {
        private readonly IRevenueDashboardAppService _dashboardAppService;

        public DashboardController(IRevenueDashboardAppService dashboardAppService)
        {
            _dashboardAppService = dashboardAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetDailyRevenueSummary(DateTime date, int? facilityId = null)
        {
            var summary = await _dashboardAppService.GetDailyRevenueSummary(date, facilityId);
            return Json(summary);
        }

        [HttpPost]
        public async Task<JsonResult> GetMonthlyRevenueByProcedureType(int month, int year, int? facilityId = null)
        {
            var revenue = await _dashboardAppService.GetMonthlyRevenueByProcedureType(month, year, facilityId);
            return Json(revenue);
        }

        [HttpPost]
        public async Task<JsonResult> GetRevenueVsQuota(int month, int year, int? facilityId = null)
        {
            var data = await _dashboardAppService.GetRevenueVsQuota(month, year, facilityId);
            return Json(data);
        }

        [HttpPost]
        public async Task<JsonResult> GetRevenueTrend(DateTime startDate, DateTime endDate, int? facilityId = null)
        {
            var trend = await _dashboardAppService.GetRevenueTrend(startDate, endDate, facilityId);
            return Json(trend);
        }
    }
}

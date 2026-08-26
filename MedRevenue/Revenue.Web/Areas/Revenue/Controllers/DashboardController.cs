using ATI.Revenue.Application.Dashboard;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using ATI.Authorization;
using ATI.Web.Controllers;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    [AbpMvcAuthorize(AppPermissions.Pages_Revenue_Dashboard)]
    public class DashboardController : ATIControllerBase
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
        public async Task<JsonResult> GetDailyRevenueSummary(DateTime? date = null, int? hospitalId = null)
        {
            var summary = await _dashboardAppService.GetDailyRevenueSummary(date, hospitalId);
            return Json(summary);
        }

        [HttpPost]
        public async Task<JsonResult> GetMonthlyRevenueByCategory(int? month = null, int? year = null, int? hospitalId = null)
        {
            var revenue = await _dashboardAppService.GetMonthlyRevenueByCategory(month, year, hospitalId);
            return Json(revenue);
        }

        [HttpPost]
        public async Task<JsonResult> GetRevenueVsQuota(int? month = null, int? year = null, int? hospitalId = null)
        {
            var data = await _dashboardAppService.GetRevenueVsQuota(month, year, hospitalId);
            return Json(data);
        }

        [HttpPost]
        public async Task<JsonResult> GetRevenueTrend(DateTime? startDate = null, DateTime? endDate = null, int? hospitalId = null)
        {
            var trend = await _dashboardAppService.GetRevenueTrend(startDate, endDate, hospitalId);
            return Json(trend);
        }
    }
}

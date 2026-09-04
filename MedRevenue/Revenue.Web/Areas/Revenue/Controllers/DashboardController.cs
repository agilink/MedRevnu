using ATI.Revenue.Application.Authorization;
using ATI.Revenue.Application.Dashboard;
using ATI.Revenue.Application.Dashboard.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using ATI.Admin.Domain.Entities;
using ATI.Authorization;
using ATI.Web.Controllers;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    [AbpMvcAuthorize(AppPermissions.Pages_Revenue_Dashboard)]
    public class DashboardController : ATIControllerBase
    {
        private readonly IRevenueDashboardAppService _dashboardAppService;
        private readonly IRepository<Facility, int> _facilityRepository;
        private readonly IPhysicianDataScopeProvider _scopeProvider;

        public DashboardController(
            IRevenueDashboardAppService dashboardAppService,
            IRepository<Facility, int> facilityRepository,
            IPhysicianDataScopeProvider scopeProvider)
        {
            _dashboardAppService = dashboardAppService;
            _facilityRepository = facilityRepository;
            _scopeProvider = scopeProvider;
        }

        public async Task<IActionResult> Index()
        {
            var today = Abp.Timing.Clock.Now.Date;
            var scope = await _scopeProvider.GetAsync();

            // A physician sees one hospital's figures, so offering the full list - and an
            // "All Hospitals" option they can never actually have - would be misleading.
            ViewBag.Hospitals = await GetHospitalSelectList(scope.IsRestricted ? scope.HospitalId : null);
            ViewBag.IsHospitalLocked = scope.IsRestricted;
            ViewBag.HasNoHospital = scope.SeesNothing;
            // Twelve whole months ending today, rather than the current month. A month-long
            // default showed an empty dashboard for most of any month - and because
            // targets are monthly, starting on the first of a month means the planned
            // figure sums twelve whole targets rather than a part-month.
            var from = new DateTime(today.Year, today.Month, 1).AddMonths(-11);

            ViewBag.DefaultFromDate = from.ToString("yyyy-MM-dd");
            ViewBag.DefaultToDate = today.ToString("yyyy-MM-dd");

            return View();
        }

        /// <summary>
        /// The whole dashboard in one request.
        /// </summary>
        /// <remarks>
        /// [FromBody] is deliberate. The older actions on this controller take simple
        /// parameters (DateTime?, int?) while the page posts a JSON body, and simple types
        /// never bind from a JSON body - so those filters silently arrived as null and the
        /// page always showed the default period. A single complex input bound from the
        /// body avoids repeating that.
        /// </remarks>
        [HttpPost]
        public async Task<JsonResult> GetDashboard([FromBody] RevenueDashboardInput input)
        {
            var data = await _dashboardAppService.GetRevenueDashboard(input);
            return Json(data);
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

        /// <summary>
        /// Every hospital, or just the one a restricted user is confined to.
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
    }
}

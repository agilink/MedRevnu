using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using ATI.Authorization;
using ATI.DashboardCustomization;
using System.Threading.Tasks;
using ATI.Web.Areas.Core.Startup;

namespace ATI.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
    public class TenantDashboardController : CustomizableDashboardControllerBase
    {
        public TenantDashboardController(DashboardViewConfiguration dashboardViewConfiguration, 
            IDashboardCustomizationAppService dashboardCustomizationAppService) 
            : base(dashboardViewConfiguration, dashboardCustomizationAppService)
        {

        }

        /// <summary>
        /// The tenant dashboard is now the Revenue dashboard.
        /// </summary>
        /// <remarks>
        /// A redirect rather than a rewrite: ASP.NET Zero's customizable-dashboard
        /// infrastructure (widget definitions, saved layouts, the host dashboard) is left
        /// intact, and a tenant without the Revenue dashboard permission still gets the
        /// stock customizable view instead of a 403 on their landing page.
        /// </remarks>
        public async Task<ActionResult> Index()
        {
            if (await IsGrantedAsync(AppPermissions.Pages_Revenue_Dashboard))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Revenue" });
            }

            return await GetView(ATIDashboardCustomizationConsts.DashboardNames.DefaultTenantDashboard);
        }
    }
}
using System.Threading.Tasks;
using Abp.Configuration;
using Microsoft.AspNetCore.Mvc;
using ATI.Configuration;
using ATI.Web.Areas.Core.Models.Layout;
using ATI.Web.Session;
using ATI.Web.Views;

namespace ATI.Web.Areas.Core.Views.Shared.Components.CoreSubscriptionBar
{
    public class CoreSubscriptionBarViewComponent : ATIViewComponent
    {
        private readonly IPerRequestSessionCache _sessionCache;

        public CoreSubscriptionBarViewComponent(
            IPerRequestSessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }

        public async Task<IViewComponentResult> InvokeAsync(string cssClass = "btn btn-icon btn-custom btn-icon-muted btn-active-light btn-active-color-primary w-35px h-35px w-md-40px h-md-40px position-relative")
        {
            var model = new SubscriptionBarViewModel
            {
                LoginInformations = await _sessionCache.GetCurrentLoginInformationsAsync(),
                SubscriptionExpireNotifyDayCount = SettingManager.GetSettingValue<int>(AppSettings.TenantManagement.SubscriptionExpireNotifyDayCount),
                CssClass = cssClass
            };

            return View(model);
        }

    }
}

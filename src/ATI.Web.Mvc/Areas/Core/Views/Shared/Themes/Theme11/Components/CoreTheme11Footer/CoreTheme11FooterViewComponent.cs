using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ATI.Web.Areas.Core.Models.Layout;
using ATI.Web.Session;
using ATI.Web.Views;

namespace ATI.Web.Areas.Core.Views.Shared.Themes.Theme11.Components.CoreTheme11Footer
{
    public class CoreTheme11FooterViewComponent : ATIViewComponent
    {
        private readonly IPerRequestSessionCache _sessionCache;

        public CoreTheme11FooterViewComponent(IPerRequestSessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var footerModel = new FooterViewModel
            {
                LoginInformations = await _sessionCache.GetCurrentLoginInformationsAsync()
            };

            return View(footerModel);
        }
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ATI.Web.Areas.Core.Models.Layout;
using ATI.Web.Session;
using ATI.Web.Views;

namespace ATI.Web.Areas.Core.Views.Shared.Themes.Theme10.Components.CoreTheme10Footer
{
    public class CoreTheme10FooterViewComponent : ATIViewComponent
    {
        private readonly IPerRequestSessionCache _sessionCache;

        public CoreTheme10FooterViewComponent(IPerRequestSessionCache sessionCache)
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

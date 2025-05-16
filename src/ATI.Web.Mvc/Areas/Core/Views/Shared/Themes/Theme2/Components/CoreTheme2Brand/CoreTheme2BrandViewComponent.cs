using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ATI.Web.Areas.Core.Models.Layout;
using ATI.Web.Session;
using ATI.Web.Views;

namespace ATI.Web.Areas.Core.Views.Shared.Themes.Theme2.Components.CoreTheme2Brand
{
    public class CoreTheme2BrandViewComponent : ATIViewComponent
    {
        private readonly IPerRequestSessionCache _sessionCache;

        public CoreTheme2BrandViewComponent(IPerRequestSessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var headerModel = new HeaderViewModel
            {
                LoginInformations = await _sessionCache.GetCurrentLoginInformationsAsync()
            };

            return View(headerModel);
        }
    }
}

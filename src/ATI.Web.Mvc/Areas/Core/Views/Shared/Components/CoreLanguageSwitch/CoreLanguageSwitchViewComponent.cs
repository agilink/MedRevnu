using System.Linq;
using System.Threading.Tasks;
using Abp.Localization;
using Microsoft.AspNetCore.Mvc;
using ATI.Web.Areas.Core.Models.Layout;
using ATI.Web.Views;

namespace ATI.Web.Areas.Core.Views.Shared.Components.CoreLanguageSwitch
{
    public class CoreLanguageSwitchViewComponent : ATIViewComponent
    {
        private readonly ILanguageManager _languageManager;

        public CoreLanguageSwitchViewComponent(ILanguageManager languageManager)
        {
            _languageManager = languageManager;
        }

        public Task<IViewComponentResult> InvokeAsync(string cssClass)
        {
            var model = new LanguageSwitchViewModel
            {
                Languages = _languageManager.GetActiveLanguages().ToList(),
                CurrentLanguage = _languageManager.CurrentLanguage,
                CssClass = cssClass
            };
            
            return Task.FromResult<IViewComponentResult>(View(model));
        }
    }
}

using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using ATI.Configuration;
using ATI.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    public abstract class RevenueBaseController : ATIControllerBase
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var currentThemeName = IocManager.Instance.Resolve<ISettingManager>()
               .GetSettingValue(AppSettings.UiManagement.Theme);
            var currentTheme = currentThemeName.ToPascalCase() ?? "Default";
            ViewBag.Layout = $"~/Themes/{currentTheme}/Views/Layout/_Layout.cshtml";
            base.OnActionExecuting(context);
        }
    }
}

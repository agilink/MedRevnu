using Abp.AspNetCore.Mvc.ViewComponents;

namespace ATI.Web.Views
{
    public abstract class ATIViewComponent : AbpViewComponent
    {
        protected ATIViewComponent()
        {
            LocalizationSourceName = ATIConsts.LocalizationSourceName;
        }
    }
}
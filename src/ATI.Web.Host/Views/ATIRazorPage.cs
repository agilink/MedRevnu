using Abp.AspNetCore.Mvc.Views;

namespace ATI.Web.Views
{
    public abstract class ATIRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected ATIRazorPage()
        {
            LocalizationSourceName = ATIConsts.LocalizationSourceName;
        }
    }
}

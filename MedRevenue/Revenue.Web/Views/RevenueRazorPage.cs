using Abp.AspNetCore.Mvc.Views;
using Abp.Localization;
using ATI;

namespace ATI.Revenue.Web.Views
{
    public abstract class RevenueRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected RevenueRazorPage()
        {
            LocalizationSourceName = ATIConsts.LocalizationSourceName;
        }
    }

    public abstract class RevenueRazorPage : RevenueRazorPage<dynamic>
    {
    }
}

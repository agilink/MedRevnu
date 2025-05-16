using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace ATI.Web.Public.Views
{
    public abstract class ATIRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected ATIRazorPage()
        {
            LocalizationSourceName = ATIConsts.LocalizationSourceName;
        }
    }
}

using Abp.AspNetCore.Mvc.Views;
using Abp.Extensions;
using Abp.Runtime.Session;
using ATI.UiCustomization;
using ATI.UiCustomization.Dto;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace ATI.Revenue.Web
{
    public abstract class RevenueRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected RevenueRazorPage()
        {
            LocalizationSourceName = ATIConsts.LocalizationSourceName;
        }


        [RazorInject] public IUiThemeCustomizerFactory UiThemeCustomizerFactory { get; set; }

        public async Task<UiCustomizationSettingsDto> GetTheme()
        {
            var themeCustomizer = await UiThemeCustomizerFactory.GetCurrentUiCustomizer();
            var settings = await themeCustomizer.GetUiSettings();
            return settings;
        }

        public async Task<string> GetContainerClass()
        {
            var theme = await GetTheme();
            if (theme.BaseSettings.Layout.LayoutType == "fluid")
            {
                return "app-container container-fluid";
            }

            return theme.BaseSettings.Layout.LayoutType.IsIn("fixed", "fluid-xxl")
                ? "app-container container-xxl"
                : "app-container container";
        }

        public async Task<string> GetLogoSkin()
        {
            var theme = await GetTheme();
            if (theme.IsTopMenuUsed || theme.IsTabMenuUsed)
            {
                return theme.BaseSettings.Layout.DarkMode ? "light" : "dark";
            }

            return theme.BaseSettings.Menu.AsideSkin;
        }
    }
}
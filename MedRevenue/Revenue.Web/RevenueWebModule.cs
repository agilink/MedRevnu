using Abp.AspNetCore.Configuration;
using Abp.AspNetZeroCore.Web;
using Abp.Dependency;
using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Resources.Embedded;
using ATI.Revenue.Application;
using ATI.Revenue.Domain;
using ATI.Revenue.EntityFrameworkCore;
using ATI.Web;
using System.Reflection;

namespace ATI.Revenue.Web
{
    [DependsOn(typeof(RevenueApplicationModule), typeof(RevenueCoreModule), typeof(ATICoreModule), typeof(ATIWebCoreModule), typeof(AbpAspNetZeroCoreWebModule), typeof(RevenueEntityFrameworkCoreModule))]
    public class RevenueWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            // Configuration.Navigation.Providers.Add<LMSNavigationProvider>();

            Configuration.Localization.Languages.Add(new LanguageInfo("en", "English", "famfamfam-flags gb", true));

            Configuration.EmbeddedResources.Sources.Add(
                new EmbeddedResourceSet(
                    "/Views/",
                    Assembly.GetExecutingAssembly(),
                    "Revenue.Views"
                )
            );

            // Must call app.UseEmbeddedFiles() at main application Configure!
            Configuration.EmbeddedResources.Sources.Add(
                new EmbeddedResourceSet(
                    "/Resources/",
                    Assembly.GetExecutingAssembly(),
                    "Revenue.Resources"
                )
            );

            // Fix for CS0246: Ensure the correct namespace is used for AbpVirtualFileSystemOptions
            Configuration.Modules.AbpAspNetCore() // This line requires the Abp.AspNetCore namespace
                .CreateControllersForAppServices(
                    typeof(RevenueApplicationModule).GetAssembly()
                );

            Configuration.EmbeddedResources.Sources.Add(new EmbeddedResourceSet("/ViewModels",
                Assembly.GetExecutingAssembly(),
                "Revenue.Views"));           
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(RevenueWebModule).GetAssembly());
        }
    }
}
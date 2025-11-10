using Abp.AspNetCore.Configuration;
using Abp.Hangfire.Configuration;
using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Resources.Embedded;
using ATI.Revenue.Application;
using ATI.Revenue.EntityFrameworkCore;
using Hangfire;
using System.Reflection;

namespace Revenue.Web
{
    [DependsOn(typeof(RevenueEntityFrameworkCoreModule), typeof(RevenueApplicationModule))]
    public class ATIRevenueWebModule : AbpModule
    {
        public override void PreInitialize()
        {
           // Configuration.Navigation.Providers.Add<CoreNavigationProvider>();


            Configuration.Localization.Languages.Add(new LanguageInfo("en", "English", "famfamfam-flags gb", true));

            Configuration.EmbeddedResources.Sources.Add(
                new EmbeddedResourceSet(
                    "/Views/",
                    Assembly.GetExecutingAssembly(),
                    "Phillips.LMS.Views"
                )
            );

            //Must call app.UseEmbeddedFiles() at main application Configure!
            Configuration.EmbeddedResources.Sources.Add(
                new EmbeddedResourceSet(
                    "/Resources/",
                    Assembly.GetExecutingAssembly(),
                    "Phillips.LMS.Resources"
                    )
                );



            Configuration.Modules.AbpAspNetCore()
             .CreateControllersForAppServices(
                 typeof(RevenueApplicationModule).GetAssembly()
             );

        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }



    }
}
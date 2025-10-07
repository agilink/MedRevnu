using Abp.AspNetCore.Configuration;
using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Resources.Embedded;
using ATI.Pharmacy.Application;
using ATI.Pharmacy.EntityFrameworkCore;
using System.Reflection;

namespace ATI.Pharmacy.Web
{
    [DependsOn(typeof(PharmacyEntityFrameworkModule),  typeof(PharmacyApplicationModule))]
    public class PharmacylWebModule : AbpModule
    {
        public override void PreInitialize()
        {
           // Configuration.Navigation.Providers.Add<LMSNavigationProvider>();

             
            Configuration.Localization.Languages.Add(new LanguageInfo("en", "English", "famfamfam-flags gb", true));

            Configuration.EmbeddedResources.Sources.Add(
                new EmbeddedResourceSet(
                    "/Views/",
                    Assembly.GetExecutingAssembly(),
                    "ATI.Pharmacy.Views"
                )
            );

            //Must call app.UseEmbeddedFiles() at main application Configure!
            Configuration.EmbeddedResources.Sources.Add(
                new EmbeddedResourceSet(
                    "/Resources/",
                    Assembly.GetExecutingAssembly(),
                    "ATI.Pharmacy.Resources"
                    )
                );



            Configuration.Modules.AbpAspNetCore()
             .CreateControllersForAppServices(
                 typeof(PharmacyApplicationModule).GetAssembly()
             );
                      
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }


       
    }
}
using Abp.AspNetCore.Configuration;
using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Resources.Embedded;
using ATI.EntityFrameworkCore;
using ATI.MedRevnu.Application;
using System.Reflection;

namespace ATI.MedRevnu.Web
{
    [DependsOn(typeof(ATIEntityFrameworkCoreModule),  typeof(MedRevnuApplicationModule))]
    public class MedRevnuWebModule : AbpModule
    {
        public override void PreInitialize()
        {
           // Configuration.Navigation.Providers.Add<LMSNavigationProvider>();

             
            Configuration.Localization.Languages.Add(new LanguageInfo("en", "English", "famfamfam-flags gb", true));

            Configuration.EmbeddedResources.Sources.Add(
                new EmbeddedResourceSet(
                    "/Views/",
                    Assembly.GetExecutingAssembly(),
                    "ATI.MedRevnu.Views"
                )
            );

            //Must call app.UseEmbeddedFiles() at main application Configure!
            Configuration.EmbeddedResources.Sources.Add(
                new EmbeddedResourceSet(
                    "/Resources/",
                    Assembly.GetExecutingAssembly(),
                    "ATI.MedRevnu.Resources"
                    )
                );



            Configuration.Modules.AbpAspNetCore()
             .CreateControllersForAppServices(
                 typeof(MedRevnuApplicationModule).GetAssembly()
             );
                      
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }


       
    }
}
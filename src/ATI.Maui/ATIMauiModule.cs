using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ATI.ApiClient;
using ATI.Maui.Core;

namespace ATI.Maui
{
    [DependsOn(typeof(ATIClientModule), typeof(AbpAutoMapperModule))]
    public class ATIMauiModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.IsEnabled = false;
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            Configuration.ReplaceService<IApplicationContext, MauiApplicationContext>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ATIMauiModule).GetAssembly());
        }
    }
}
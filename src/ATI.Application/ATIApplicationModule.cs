using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ATI.Authorization;

namespace ATI
{
    /// <summary>
    /// Application layer module of the application.
    /// </summary>
    [DependsOn(
        typeof(ATIApplicationSharedModule),
        typeof(ATICoreModule)
        )]
    public class ATIApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ATIApplicationModule).GetAssembly());
        }
    }
}
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ATI.Revenue.Domain;

namespace ATI.Revenue.Application
{
    [DependsOn(
        typeof(RevenueCoreModule),
        typeof(AbpAutoMapperModule)
    )]
    public class RevenueApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<RevenueAuthorizationProvider>();
            
            Configuration.Modules.AbpAutoMapper().Configurators.Add(RevenueDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(RevenueApplicationModule).GetAssembly());
        }
    }
}
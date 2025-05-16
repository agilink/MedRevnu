using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace ATI.Startup
{
    [DependsOn(typeof(ATICoreModule))]
    public class ATIGraphQLModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ATIGraphQLModule).GetAssembly());
        }

        public override void PreInitialize()
        {
            base.PreInitialize();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }
    }
}
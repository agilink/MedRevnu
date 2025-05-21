
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ATI;

namespace ATI.MedRevnu.Application
{
    [DependsOn(typeof(ATICoreModule), typeof(ATIApplicationModule))]
    public class MedRevnuApplicationModule : AbpModule
    {
       
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(MedRevnuDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {

            IocManager.RegisterAssemblyByConvention(typeof(MedRevnuApplicationModule).GetAssembly());
         
        }
        public override void PostInitialize()
        {
           
                        
        }

      
    }
}
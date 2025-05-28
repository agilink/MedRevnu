
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ATI;
using ATI.Admin.Application;
using ATI.Admin.Domain;



namespace ATI.Admin.Application
{
    [DependsOn(typeof(AdminCoreModule), typeof(ATIApplicationModule))]
    public class AdminApplicationModule : AbpModule
    {
       
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(AdminDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {

            IocManager.RegisterAssemblyByConvention(typeof(AdminApplicationModule).GetAssembly());
         
        }
        public override void PostInitialize()
        {          
                        
        }

      
    }
}

using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ATI;
using ATI.Admin.Application;
using ATI.Pharmacy.Domain;



namespace ATI.Pharmacy.Application
{
    [DependsOn(typeof(PharmacyCoreModule), typeof(ATIApplicationModule), typeof(AdminApplicationModule))]
    public class PharmacyApplicationModule : AbpModule
    {
       
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(PharmacyDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {

            IocManager.RegisterAssemblyByConvention(typeof(PharmacyApplicationModule).GetAssembly());
         
        }
        public override void PostInitialize()
        {
           
                        
        }

      
    }
}
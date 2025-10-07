using Abp;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using Abp.EntityFramework;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ATI.EntityFrameworkCore;
using ATI.EntityHistory;
using ATI.Pharmacy.Domain;
using ATI.Pharmacy.EntityFrameworkCore;

namespace ATI.Pharmacy.EntityFrameworkCore
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule), typeof(PharmacyCoreModule), typeof(ATIEntityFrameworkCoreModule),  typeof(ATICoreModule))]
    public class PharmacyEntityFrameworkModule : AbpModule
    {
        public override void PreInitialize()
        {
           
            Configuration.Modules.AbpEfCore().AddDbContext<PharmacyModuleDbContext>(options =>
            {
                if (options.ExistingConnection != null)
                {
                    DbContextOptionsConfigurer.Configure(options.DbContextOptions,
                        options.ExistingConnection);
                }
                else
                {
                    DbContextOptionsConfigurer.Configure(options.DbContextOptions,
                        options.ConnectionString);
                }
            });

            Configuration.Auditing.IsEnabled = true;
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;
            Configuration.EntityHistory.IsEnabled = true;
            Configuration.EntityHistory.Selectors.Add(new NamedTypeSelector("FullAuditedEntity", type => typeof(IFullAudited).IsAssignableFrom(type)));
            
            Configuration.CustomConfigProviders.Add(new EntityHistoryConfigProvider(Configuration));

            Configuration.Modules.AbpAutoMapper().Configurators.Add(PharmacyAuditMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(PharmacyEntityFrameworkModule).GetAssembly());
        }
    }
}
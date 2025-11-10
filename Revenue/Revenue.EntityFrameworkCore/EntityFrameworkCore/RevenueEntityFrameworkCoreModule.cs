using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using ATI.EntityFrameworkCore;
using ATI.Revenue.Domain;

namespace ATI.Revenue.EntityFrameworkCore
{
    [DependsOn(
        typeof(RevenueCoreModule),
        typeof(ATIEntityFrameworkCoreModule)
    )]
    public class RevenueEntityFrameworkCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpEfCore().AddDbContext<RevenueModuleDbContext>(options =>
            {
                if (options.ExistingConnection != null)
                {
                    RevenueModuleDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                }
                else
                {
                    RevenueModuleDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                }
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(RevenueEntityFrameworkCoreModule).GetAssembly());
        }
    }
}
using Abp.Modules;
using Abp.Reflection.Extensions;
using ATI.Admin.Domain;

namespace ATI.Revenue.Domain
{
    [DependsOn(typeof(AdminCoreModule))]
    public class RevenueCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(RevenueCoreModule).GetAssembly());
        }
    }
}
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero;
using ATI.Admin.Domain;

namespace ATI.Revenue.Domain
{
    [DependsOn(typeof(AbpZeroCoreModule), typeof(ATICoreModule), typeof(ATICoreSharedModule))]
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
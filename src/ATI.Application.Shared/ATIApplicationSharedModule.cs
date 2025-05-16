using Abp.Modules;
using Abp.Reflection.Extensions;

namespace ATI
{
    [DependsOn(typeof(ATICoreSharedModule))]
    public class ATIApplicationSharedModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ATIApplicationSharedModule).GetAssembly());
        }
    }
}
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace ATI
{
    public class ATICoreSharedModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ATICoreSharedModule).GetAssembly());
        }
    }
}
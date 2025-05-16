using Abp.Modules;
using Abp.Reflection.Extensions;

namespace ATI
{
    public class ATIClientModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ATIClientModule).GetAssembly());
        }
    }
}

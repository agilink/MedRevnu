using Abp.Modules;
using ATI;

namespace ATI.Admin.Domain
{
    [DependsOn(typeof(ATICoreModule))]
    public class AdminCoreModule:AbpModule
    {
        public override void PreInitialize()
        {         
        }
    }
}
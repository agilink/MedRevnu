using Abp.Modules;
using ATI;

namespace ATI.Pharmacy.Domain
{
    [DependsOn(typeof(ATICoreModule))]
    public class PharmacyCoreModule:AbpModule
    {
        public override void PreInitialize()
        {         
        }
    }
}
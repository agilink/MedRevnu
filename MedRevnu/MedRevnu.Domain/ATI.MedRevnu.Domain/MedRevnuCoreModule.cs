using Abp.Modules;
using ATI.MedRevnu.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATI.MedRevnu.Domain
{
    [DependsOn(typeof(ATICoreModule))]
    public class MedRevnuCoreModule : AbpModule
    {
        public override void Initialize()
        {
            // Register domain services
            IocManager.Register<QuotaCalculationService>();
        }
    }
}

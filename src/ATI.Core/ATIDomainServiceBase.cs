using Abp.Domain.Services;

namespace ATI
{
    public abstract class ATIDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */

        protected ATIDomainServiceBase()
        {
            LocalizationSourceName = ATIConsts.LocalizationSourceName;
        }
    }
}

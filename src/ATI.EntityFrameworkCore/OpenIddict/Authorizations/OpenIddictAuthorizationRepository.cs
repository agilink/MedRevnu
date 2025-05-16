using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.OpenIddict.EntityFrameworkCore.Authorizations;
using ATI.EntityFrameworkCore;

namespace ATI.OpenIddict.Authorizations
{
    public class OpenIddictAuthorizationRepository : EfCoreOpenIddictAuthorizationRepository<ATIDbContext>
    {
        public OpenIddictAuthorizationRepository(
            IDbContextProvider<ATIDbContext> dbContextProvider,
            IUnitOfWorkManager unitOfWorkManager) : base(dbContextProvider, unitOfWorkManager)
        {
        }
    }
}
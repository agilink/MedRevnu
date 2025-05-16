using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.OpenIddict.EntityFrameworkCore.Scopes;
using ATI.EntityFrameworkCore;

namespace ATI.OpenIddict.Scopes
{
    public class OpenIddictScopeRepository : EfCoreOpenIddictScopeRepository<ATIDbContext>
    {
        public OpenIddictScopeRepository(
            IDbContextProvider<ATIDbContext> dbContextProvider,
            IUnitOfWorkManager unitOfWorkManager) : base(dbContextProvider, unitOfWorkManager)
        {
        }
    }
}
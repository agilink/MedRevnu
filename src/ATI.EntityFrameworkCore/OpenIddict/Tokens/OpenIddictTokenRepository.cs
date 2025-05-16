using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.OpenIddict.EntityFrameworkCore.Tokens;
using ATI.EntityFrameworkCore;

namespace ATI.OpenIddict.Tokens
{
    public class OpenIddictTokenRepository : EfCoreOpenIddictTokenRepository<ATIDbContext>
    {
        public OpenIddictTokenRepository(
            IDbContextProvider<ATIDbContext> dbContextProvider,
            IUnitOfWorkManager unitOfWorkManager) : base(dbContextProvider, unitOfWorkManager)
        {
        }
    }
}
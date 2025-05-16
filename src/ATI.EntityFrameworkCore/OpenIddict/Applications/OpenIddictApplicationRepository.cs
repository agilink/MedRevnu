using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.OpenIddict.EntityFrameworkCore.Applications;
using ATI.EntityFrameworkCore;

namespace ATI.OpenIddict.Applications
{
    public class OpenIddictApplicationRepository : EfCoreOpenIddictApplicationRepository<ATIDbContext>
    {
        public OpenIddictApplicationRepository(
            IDbContextProvider<ATIDbContext> dbContextProvider,
            IUnitOfWorkManager unitOfWorkManager) : base(dbContextProvider, unitOfWorkManager)
        {
        }
    }
}
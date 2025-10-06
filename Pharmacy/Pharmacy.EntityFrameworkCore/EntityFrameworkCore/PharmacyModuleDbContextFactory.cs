
using ATI.Configuration;
using ATI.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace ATI.Pharmacy.EntityFrameworkCore
{
    /* This class is needed to run EF Core PMC commands. Not used anywhere else  */
    public class PharmacyModuleDbContextFactory : IDesignTimeDbContextFactory<PharmacyModuleDbContext>
    {
        public PharmacyModuleDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PharmacyModuleDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            DbContextOptionsConfigurer.Configure(
                builder,
                configuration.GetConnectionString("Default")
            );

            return new PharmacyModuleDbContext(builder.Options);
        }
    }
}
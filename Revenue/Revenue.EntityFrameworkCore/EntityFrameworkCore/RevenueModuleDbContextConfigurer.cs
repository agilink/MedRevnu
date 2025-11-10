using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using ATI.Configuration;
using ATI.Web;

namespace ATI.Revenue.EntityFrameworkCore
{
    public static class RevenueModuleDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<RevenueModuleDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<RevenueModuleDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }

    public class RevenueModuleDbContextFactory : IDesignTimeDbContextFactory<RevenueModuleDbContext>
    {
        public RevenueModuleDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<RevenueModuleDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            RevenueModuleDbContextConfigurer.Configure(
                builder,
                configuration.GetConnectionString(ATIConsts.ConnectionStringName)
            );

            return new RevenueModuleDbContext(builder.Options);
        }
    }
}
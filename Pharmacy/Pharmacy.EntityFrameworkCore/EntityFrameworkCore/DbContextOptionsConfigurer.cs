using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace ATI.Pharmacy.EntityFrameworkCore
{
    public static class DbContextOptionsConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<PharmacyModuleDbContext> builder, string connectionString)
        {            
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<PharmacyModuleDbContext> builder, DbConnection connection)
        {            
            builder.UseSqlServer(connection);
        }
    }
}

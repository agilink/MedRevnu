using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace ATI.EntityFrameworkCore
{
    public static class ATIDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<ATIDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<ATIDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
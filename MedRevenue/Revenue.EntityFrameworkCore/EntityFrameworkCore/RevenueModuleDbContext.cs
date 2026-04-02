using Abp.EntityFrameworkCore;
using ATI.Revenue.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ATI.Revenue.EntityFrameworkCore
{
    public class RevenueModuleDbContext : AbpDbContext
    {
        public virtual DbSet<Case> Cases { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<CaseProduct> CaseProducts { get; set; }
        public virtual DbSet<ProcedureType> ProcedureTypes { get; set; }
        public virtual DbSet<ProcedureQuota> ProcedureQuotas { get; set; }

        public RevenueModuleDbContext(DbContextOptions<RevenueModuleDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Case entity configuration
            modelBuilder.Entity<Case>(entity =>
            {
                entity.ToTable("Case", "REV");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CaseNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ClientName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.SurgeonName).HasMaxLength(200);

                entity.HasMany(e => e.CaseProducts)
                    .WithOne(e => e.Case)
                    .HasForeignKey(e => e.CaseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ProcedureType)
                    .WithMany(e => e.Cases)
                    .HasForeignKey(e => e.ProcedureTypeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Product entity configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product", "REV");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Manufacturer).HasMaxLength(200);
                entity.Property(e => e.ModelNo).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Cost).HasPrecision(18, 2);
                entity.Property(e => e.Price).HasPrecision(18, 2);

                entity.HasOne(e => e.ProductCategory)
                    .WithMany(e => e.Products)
                    .HasForeignKey(e => e.ProductCategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ProductCategory entity configuration
            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("ProductCategory", "REV");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // CaseProduct entity configuration
            modelBuilder.Entity<CaseProduct>(entity =>
            {
                entity.ToTable("CaseProduct", "REV");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.Discount).HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);

                entity.HasOne(e => e.Case)
                    .WithMany(e => e.CaseProducts)
                    .HasForeignKey(e => e.CaseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(e => e.CaseProducts)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ProcedureType entity configuration
            modelBuilder.Entity<ProcedureType>(entity =>
            {
                entity.ToTable("ProcedureType", "REV");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CategoryGroup).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.DisplayOrder).IsRequired();

                entity.HasIndex(e => e.Code).IsUnique();

                entity.HasMany(e => e.Cases)
                    .WithOne(e => e.ProcedureType)
                    .HasForeignKey(e => e.ProcedureTypeId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(e => e.ProcedureQuotas)
                    .WithOne(e => e.ProcedureType)
                    .HasForeignKey(e => e.ProcedureTypeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ProcedureQuota entity configuration
            modelBuilder.Entity<ProcedureQuota>(entity =>
            {
                entity.ToTable("ProcedureQuota", "REV");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProcedureTypeId).IsRequired();
                entity.Property(e => e.QuotaPeriod).IsRequired();
                entity.Property(e => e.QuotaValue).HasPrecision(18, 2).IsRequired();
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate).IsRequired();
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasOne(e => e.ProcedureType)
                    .WithMany(e => e.ProcedureQuotas)
                    .HasForeignKey(e => e.ProcedureTypeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
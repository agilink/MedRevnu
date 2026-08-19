using Abp.EntityFrameworkCore;
using ATI.Revenue.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ATI.Revenue.EntityFrameworkCore
{
    public class RevenueModuleDbContext : AbpDbContext
    {
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProcedureType> ProcedureTypes { get; set; }

        public RevenueModuleDbContext(DbContextOptions<RevenueModuleDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
            });
        }
    }
}
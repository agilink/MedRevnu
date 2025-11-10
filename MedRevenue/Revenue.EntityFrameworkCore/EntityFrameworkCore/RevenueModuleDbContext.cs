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

                entity.HasMany(e => e.CaseProducts)
                    .WithOne(e => e.Case)
                    .HasForeignKey(e => e.CaseId)
                    .OnDelete(DeleteBehavior.Cascade);
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
        }
    }
}
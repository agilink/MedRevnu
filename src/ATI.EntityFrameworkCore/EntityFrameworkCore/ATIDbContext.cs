using Abp.OpenIddict.Applications;
using Abp.OpenIddict.Authorizations;
using Abp.OpenIddict.EntityFrameworkCore;
using Abp.OpenIddict.Scopes;
using Abp.OpenIddict.Tokens;
using Abp.Zero.EntityFrameworkCore;
using ATI.Admin.Domain.Entities;
using ATI.Authorization.Delegation;
using ATI.Authorization.Roles;
using ATI.Authorization.Users;
using ATI.Chat;
using ATI.Editions;
using ATI.ExtraProperties;
using ATI.Friendships;
using ATI.MultiTenancy;
using ATI.MultiTenancy.Accounting;
using ATI.MultiTenancy.Payments;
using ATI.Revenue.Domain.Entities;
using ATI.Storage;
using Castle.MicroKernel;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace ATI.EntityFrameworkCore
{
    public class ATIDbContext : AbpZeroDbContext<Tenant, Role, User, ATIDbContext>, IOpenIddictDbContext
    {
        /* Define an IDbSet for each entity of the application */

        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Admin.Domain.Entities.Address> Address { get; set; }
        public virtual DbSet<Facility> Facility { get; set; }
        public virtual DbSet<UserCompany> UserCompany { get; set; }
        public virtual DbSet<CompanyType> CompanyType { get; set; }
        public virtual DbSet<UomType> UomType { get; set; }
        public virtual DbSet<Uom> Uom { get; set; }
        public virtual DbSet<Personnel> Personnel { get; set; }

        // Revenue module entities
        public virtual DbSet<Case> Cases { get; set; }
        public virtual DbSet<Revenue.Domain.Entities.Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductSubcategory> ProductSubcategories { get; set; }
        public virtual DbSet<CaseProduct> CaseProducts { get; set; }
        public virtual DbSet<ProcedureType> ProcedureTypes { get; set; }
        public virtual DbSet<ProcedureQuota> ProcedureQuotas { get; set; }
        public virtual DbSet<ProcedureTransaction> ProcedureTransactions { get; set; }
        public virtual DbSet<ProductQuota> ProductQuotas { get; set; }
        public virtual DbSet<HospitalProductPrice> HospitalProductPrices { get; set; }

        public virtual DbSet<OpenIddictApplication> Applications { get; }
        
        public virtual DbSet<OpenIddictAuthorization> Authorizations { get; }
        
        public virtual DbSet<OpenIddictScope> Scopes { get; }
        
        public virtual DbSet<OpenIddictToken> Tokens { get; }
        
        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        public virtual DbSet<Friendship> Friendships { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

        public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }
        
        public virtual DbSet<SubscriptionPaymentProduct> SubscriptionPaymentProducts { get; set; }

        public virtual DbSet<MultiTenancy.Accounting.Invoice> Invoices { get; set; }

        public virtual DbSet<UserDelegation> UserDelegations { get; set; }

        public virtual DbSet<RecentPassword> RecentPasswords { get; set; }

        public ATIDbContext(DbContextOptions<ATIDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BinaryObject>(b => { b.HasIndex(e => new { e.TenantId }); });

            modelBuilder.Entity<SubscriptionPayment>(x =>
            {
                x.Property(u => u.ExtraProperties)
                    .HasConversion(
                        d => JsonSerializer.Serialize(d, new JsonSerializerOptions()
                        {
                            WriteIndented = false
                        }),
                        s => JsonSerializer.Deserialize<ExtraPropertyDictionary>(s, new JsonSerializerOptions()
                        {
                            WriteIndented = false
                        })
                    );
            });
            
            modelBuilder.Entity<SubscriptionPaymentProduct>(x =>
            {
                x.Property(u => u.ExtraProperties)
                    .HasConversion(
                        d => JsonSerializer.Serialize(d, new JsonSerializerOptions()
                        {
                            WriteIndented = false
                        }),
                        s => JsonSerializer.Deserialize<ExtraPropertyDictionary>(s, new JsonSerializerOptions()
                        {
                            WriteIndented = false
                        })
                    );
            });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId, e.ReadState });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.UserId, e.ReadState });
            });

            modelBuilder.Entity<Friendship>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.FriendUserId });
                b.HasIndex(e => new { e.FriendTenantId, e.UserId });
                b.HasIndex(e => new { e.FriendTenantId, e.FriendUserId });
            });

            modelBuilder.Entity<Tenant>(b =>
            {
                b.HasIndex(e => new { e.SubscriptionEndDateUtc });
                b.HasIndex(e => new { e.CreationTime });
            });

            modelBuilder.Entity<SubscriptionPayment>(b =>
            {
                b.HasIndex(e => new { e.Status, e.CreationTime });
                b.HasIndex(e => new { PaymentId = e.ExternalPaymentId, e.Gateway });
            });

            modelBuilder.Entity<UserDelegation>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.SourceUserId });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId });
            });


             // Add this method to the ATIDbContext class
             // Replace the selected code in OnModelCreating with this call
             ModelCreatingAdminModule(ref modelBuilder);
             ModelCreatingRevenueModule(ref modelBuilder);


            modelBuilder.ConfigureOpenIddict();
        }

        private void ModelCreatingAdminModule(ref ModelBuilder modelBuilder)
        {
            var admSchema = "ADM";
            modelBuilder.Entity<Admin.Domain.Entities.Address>(b =>
            {
                b.ToTable("Address", admSchema);
            });
            modelBuilder.Entity<Admin.Domain.Entities.Company>(b =>
            {
                b.ToTable("Company", admSchema);
            });
            modelBuilder.Entity<Admin.Domain.Entities.Facility>(b =>
            {
                b.ToTable("Facility", admSchema);
            });
            modelBuilder.Entity<Admin.Domain.Entities.State>(b =>
            {
                b.ToTable("State", admSchema);
            });
            modelBuilder.Entity<Admin.Domain.Entities.UserCompany>(b =>
            {
                b.ToTable("UserCompany", admSchema);
            });
            modelBuilder.Entity<Admin.Domain.Entities.UomType>(b =>
            {
                b.ToTable("UomType", admSchema);
            });
            modelBuilder.Entity<Admin.Domain.Entities.Uom>(b =>
            {
                b.ToTable("Uom", admSchema);
            });

            // Personnel entity configuration
            modelBuilder.Entity<Admin.Domain.Entities.Personnel>(entity =>
            {
                entity.ToTable("Personnel", admSchema);
                entity.HasKey(e => e.Id);

                // String properties with max lengths
                entity.Property(e => e.FIRST_NAME).HasMaxLength(100);
                entity.Property(e => e.FIRST_NAME_PROPER).HasMaxLength(100);
                entity.Property(e => e.MIDDLE_NAME).HasMaxLength(100);
                entity.Property(e => e.LAST_NAME).HasMaxLength(100);
                entity.Property(e => e.SSN).HasMaxLength(11);
                entity.Property(e => e.EMPLOYEE_ID).HasMaxLength(50);
                entity.Property(e => e.COMPANY_NAME).HasMaxLength(200);
                entity.Property(e => e.ADDRESS1).HasMaxLength(200);
                entity.Property(e => e.ADDRESS2).HasMaxLength(200);
                entity.Property(e => e.CITY).HasMaxLength(100);
                entity.Property(e => e.ZIP_CODE).HasMaxLength(10);
                entity.Property(e => e.NUMBER_HOME).HasMaxLength(20);
                entity.Property(e => e.NUMBER_MOBILE).HasMaxLength(20);
                entity.Property(e => e.EMAIL_WORK).HasMaxLength(200);
                entity.Property(e => e.EMAIL_OTHER).HasMaxLength(200);
                entity.Property(e => e.NOTES).HasMaxLength(2000);
                entity.Property(e => e.MODIFIED_BY).HasMaxLength(100);

                // Enum properties stored as integers
                entity.Property(e => e.PersonnelTypeID).HasConversion<int?>();
                entity.Property(e => e.EmployeeStatusID).HasConversion<int?>();

                // Foreign key relationships
                entity.HasOne(e => e.Facility)
                    .WithMany()
                    .HasForeignKey(e => e.FacilityId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.State)
                    .WithMany()
                    .HasForeignKey(e => e.StateID)
                    .OnDelete(DeleteBehavior.SetNull);

                // Indexes for performance
                entity.HasIndex(e => e.EMPLOYEE_ID);
                entity.HasIndex(e => e.FacilityId);
                entity.HasIndex(e => e.LAST_NAME);
            });

            // Add additional Admin.Domain.Entities here as needed
        }

        private void ModelCreatingRevenueModule(ref ModelBuilder modelBuilder)
        {
            var revSchema = "REV";

            // Case entity configuration
            modelBuilder.Entity<Case>(entity =>
            {
                entity.ToTable("Case", revSchema);
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
            modelBuilder.Entity<Revenue.Domain.Entities.Product>(entity =>
            {
                entity.ToTable("Product", revSchema);
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductCode).HasMaxLength(30);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.BasePrice).HasPrecision(10, 2);
                entity.Property(e => e.Manufacturer).HasMaxLength(200);
                entity.Property(e => e.ModelNo).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Cost).HasPrecision(18, 2);
                entity.Property(e => e.Price).HasPrecision(18, 2);

                entity.HasOne(e => e.ProductCategory)
                    .WithMany(e => e.Products)
                    .HasForeignKey(e => e.ProductCategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.ProductSubcategory)
                    .WithMany(e => e.Products)
                    .HasForeignKey(e => e.SubproductCategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.ProductCode);
            });

            // ProductCategory entity configuration
            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("ProductCategory", revSchema);
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ShortDescription).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);

                // Explicitly configure audit fields to handle NULL values
                entity.Property(e => e.CreationTime).IsRequired();
                entity.Property(e => e.LastModificationTime).IsRequired(false);
            });

            // ProductSubcategory entity configuration
            modelBuilder.Entity<ProductSubcategory>(entity =>
            {
                entity.ToTable("ProductSubcategory", revSchema);
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductCategoryId).IsRequired();
                entity.Property(e => e.SubcategoryName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ImplantType).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasOne(e => e.ProductCategory)
                    .WithMany(e => e.ProductSubcategories)
                    .HasForeignKey(e => e.ProductCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CaseProduct entity configuration
            modelBuilder.Entity<CaseProduct>(entity =>
            {
                entity.ToTable("CaseProduct", revSchema);
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
                entity.ToTable("ProcedureType", revSchema);
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
                entity.ToTable("ProcedureQuota", revSchema);
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

            // ProcedureTransaction entity configuration
            modelBuilder.Entity<ProcedureTransaction>(entity =>
            {
                entity.ToTable("ProcedureTransaction", revSchema);
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProcedureDate).IsRequired();
                entity.Property(e => e.PhysicianId).IsRequired();
                entity.Property(e => e.ProductId).IsRequired();
                entity.Property(e => e.ImplantType).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.UnitPrice).HasPrecision(10, 2).IsRequired();
                entity.Property(e => e.TotalAmount).HasPrecision(10, 2);

                entity.HasIndex(e => e.ProcedureDate);
                entity.HasIndex(e => e.PhysicianId);
                entity.HasIndex(e => e.HospitalId);
            });

            // ProductQuota entity configuration
            modelBuilder.Entity<ProductQuota>(entity =>
            {
                entity.ToTable("ProductQuota", revSchema);
                entity.HasKey(e => e.Id);
                entity.Property(e => e.HospitalId).IsRequired();
                entity.Property(e => e.ProductCategoryId).IsRequired();
                entity.Property(e => e.PeriodMonth).IsRequired();
                entity.Property(e => e.PeriodYear).IsRequired();
                entity.Property(e => e.TargetAmount).HasPrecision(10, 2).IsRequired();

                entity.HasOne(e => e.Hospital)
                    .WithMany()
                    .HasForeignKey(e => e.HospitalId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ProductCategory)
                    .WithMany()
                    .HasForeignKey(e => e.ProductCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => new { e.HospitalId, e.ProductCategoryId, e.PeriodMonth, e.PeriodYear });
            });

            // HospitalProductPrice entity configuration
            modelBuilder.Entity<HospitalProductPrice>(entity =>
            {
                entity.ToTable("HospitalProductPrice", revSchema);
                entity.HasKey(e => e.Id);
                entity.Property(e => e.HospitalId).IsRequired();
                entity.Property(e => e.ProductId).IsRequired();
                entity.Property(e => e.ProductCode).HasMaxLength(100);
                entity.Property(e => e.UnitPrice).HasPrecision(10, 2).IsRequired();
                entity.Property(e => e.EffectiveDate).IsRequired();

                entity.HasOne(e => e.Hospital)
                    .WithMany()
                    .HasForeignKey(e => e.HospitalId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.HospitalId);
                entity.HasIndex(e => e.ProductId);
                entity.HasIndex(e => new { e.HospitalId, e.ProductId, e.EffectiveDate });
            });
        }
    }
}
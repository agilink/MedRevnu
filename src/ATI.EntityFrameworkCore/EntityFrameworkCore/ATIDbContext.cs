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
            // Add additional Admin.Domain.Entities here as needed
        }
    }
}
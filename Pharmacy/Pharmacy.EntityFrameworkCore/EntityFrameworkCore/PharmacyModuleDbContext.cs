using Abp.Authorization.Users;
using Abp.Domain.Entities;
using Abp.EntityFrameworkCore;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Zero.EntityFrameworkCore;
using ATI.Admin.Domain.Entities;
using ATI.Authorization.Roles;
using ATI.Authorization.Users;
using ATI.EntityFrameworkCore;
using ATI.Pharmacy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Stripe.Billing;

namespace ATI.Pharmacy.EntityFrameworkCore
{
    [MultiTenancySide(MultiTenancySides.Tenant)]
    public class PharmacyModuleDbContext : AbpDbContext
    {
        public virtual DbSet<UserCompanyDoctor> UserCompanyDoctor { get;set;}
        public virtual DbSet<Allergy> Allergy { get; set; }
        public virtual DbSet<FormulationType> FormulationType { get; set; }
        public virtual DbSet<DosageStrength> DosageStrength { get; set; }
        public virtual DbSet<DosageRoute> DosageRoute { get; set; }
        public virtual DbSet<DosageFrequency> DosageFrequency { get; set; }
        public virtual DbSet<DosageForm> DosageForms { get; set; }
        public virtual DbSet<DosageDuration> DosageDuration { get; set; }
        public virtual DbSet<Doctor> Doctor { get; set; }
        public virtual DbSet<Contraindication> Contraindication { get; set; }
        public virtual DbSet<Insurance> Insurance { get; set; }
        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<Medication> Medication { get; set; }
        public virtual DbSet<Patient> Patient { get; set; }
        public virtual DbSet<PatientAllergy> PatientAllergy { get; set; }
        public virtual DbSet<Prescription> Prescription { get; set; }
        public virtual DbSet<PrescriptionHistory> PrescriptionHistory { get; set; }
        public virtual DbSet<PrescriptionItem> PrescriptionItem { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        //public virtual DbSet<DUser> DUser { get; set; }


        public PharmacyModuleDbContext(DbContextOptions<PharmacyModuleDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("PHARM");
            string defaultSchema = "PHARM";

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.ToTable("Doctor", defaultSchema); // Table name
                entity.HasKey(d => d.Id); // Primary key
                // Foreign key relationship to AbpUsers
                entity.HasOne(d => d.User)
                      .WithMany() // Adjust based on your relationship type
                      .HasForeignKey(d => d.UserId)
                      .HasConstraintName("FK_Doctor_AbpUsers_UserId") // Custom FK name
                      .IsRequired(); // Ensure FK is mandatory
                                     //

                //entity.HasOne(d => d.DUser)
                //.WithMany()
                //.HasForeignKey(d => d.UserId)
                //.IsRequired(false); // Ensure FK is mandatory

                //entity.HasMany(d => d.UserCompanies)
                //.WithOne()
                //.HasForeignKey(uc => uc.UserId)
                //.OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patient", defaultSchema); // Table name
                entity.HasKey(d => d.Id); // Primary key
                // Foreign key relationship to AbpUsers
                entity.HasOne(d => d.User)
                      .WithMany() // Adjust based on your relationship type
                      .HasForeignKey(d => d.UserId)
                      .HasConstraintName("FK_Patient_AbpUsers_UserId") // Custom FK name
                      .IsRequired(); // Ensure FK is mandatory

                entity.HasOne(d => d.Address)
                  .WithMany() // One-to-many: one Address can have many Patients
                  .HasForeignKey(d => d.AddressId);
            });





            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.ToTable("Prescription", defaultSchema); // Table name
                entity.HasKey(d => d.Id); // Primary key
                // Foreign key relationship to Facility
                entity.HasOne(d => d.PrescriberFacility)
                      .WithMany() // Adjust based on your relationship type
                      .HasForeignKey(d => d.PrescriberFacilityId);
                //.HasConstraintName("FK_Patient_AbpUsers_UserId") // Custom FK name
                //.IsRequired(); // Ensure FK is mandatory
            });

            modelBuilder.Entity<User>(user =>
            {
                user.ToTable("AbpUsers", "public"); // Map AbpUsers to dbo schema
                user.Ignore(u => u.CreatorUser);
                user.Ignore(u => u.DeleterUser);
                user.Ignore(u => u.LastModifierUser);
            });

            modelBuilder.Entity<Facility>(facility =>
            {
                facility.ToTable("Facility", "ADM"); // Map Facility to pharm
                //facility.HasOne(d => d.Address)
                //  .WithMany() // One-to-many: one Address can have many Facility
                //  .HasForeignKey(d => d.AddressId);

            });

            modelBuilder.Entity<Medication>()
            .HasQueryFilter(u => true);

            modelBuilder.Entity<Address>(address =>
            {
                address.ToTable("Address", "ADM"); // Map Address to pharm

            });

            modelBuilder.Entity<State>(state =>
            {
                state.ToTable("State", "ADM"); // Map State to pharm
            });
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    // max char length value in sqlserver
                    if (property.GetMaxLength() == 67108864)
                        // max char length value in postgresql
                        property.SetMaxLength(10485760);
                }
            }
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(), // Convert to UTC on save
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));        // Convert back with UTC kind
                    }
                }
            }
        }

    }

}

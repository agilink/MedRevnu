using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATI.Pharmacy.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class initialPharmacyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PHARM");

            //migrationBuilder.EnsureSchema(
            //    name: "public");

            migrationBuilder.EnsureSchema(
                name: "ADM");

            //migrationBuilder.CreateTable(
            //    name: "AbpUsers",
            //    schema: "public",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ProfilePictureId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            //        ShouldChangePasswordOnNextLogin = table.Column<bool>(type: "bit", nullable: false),
            //        SignInTokenExpireTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        SignInToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        GoogleAuthenticatorKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        RecoveryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
            //        LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //        DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
            //        DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        AuthenticationSource = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
            //        UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //        TenantId = table.Column<int>(type: "int", nullable: true),
            //        EmailAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
            //        Surname = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
            //        Password = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //        EmailConfirmationCode = table.Column<string>(type: "nvarchar(328)", maxLength: 328, nullable: true),
            //        PasswordResetCode = table.Column<string>(type: "nvarchar(328)", maxLength: 328, nullable: true),
            //        LockoutEndDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        AccessFailedCount = table.Column<int>(type: "int", nullable: false),
            //        IsLockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
            //        PhoneNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
            //        IsPhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
            //        SecurityStamp = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
            //        IsTwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
            //        IsEmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //        NormalizedEmailAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //        ConcurrencyStamp = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AbpUsers", x => x.Id);
            //    });

            migrationBuilder.CreateTable(
                name: "Allergy",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllergyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reaction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allergy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DosageDuration",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DosageDuration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DosageForms",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DosageForms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DosageFrequency",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DosageFrequency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DosageRoute",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DosageRoute", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DosageStrength",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DosageStrength", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormulationType",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormulationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Insurance",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    InsuranceProvider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PolicyNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverageDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PCN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Group = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insurance", x => x.Id);
                });

            //migrationBuilder.CreateTable(
            //    name: "State",
            //    schema: "ADM",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        isActive = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_State", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UomType",
            //    schema: "PHARM",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TenantId = table.Column<int>(type: "int", nullable: true),
            //        Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UomType", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AbpPermissions",
            //    schema: "PHARM",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<long>(type: "bigint", nullable: false),
            //        CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
            //        TenantId = table.Column<int>(type: "int", nullable: true),
            //        Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //        IsGranted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AbpPermissions", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_AbpPermissions_AbpUsers_UserId",
            //            column: x => x.UserId,
            //            principalSchema: "dbo",
            //            principalTable: "AbpUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AbpSettings",
            //    schema: "PHARM",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TenantId = table.Column<int>(type: "int", nullable: true),
            //        UserId = table.Column<long>(type: "bigint", nullable: true),
            //        Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //        Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
            //        LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AbpSettings", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_AbpSettings_AbpUsers_UserId",
            //            column: x => x.UserId,
            //            principalSchema: "dbo",
            //            principalTable: "AbpUsers",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AbpUserClaims",
            //    schema: "PHARM",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TenantId = table.Column<int>(type: "int", nullable: true),
            //        UserId = table.Column<long>(type: "bigint", nullable: false),
            //        ClaimType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //        ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AbpUserClaims", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_AbpUserClaims_AbpUsers_UserId",
            //            column: x => x.UserId,
            //            principalSchema: "dbo",
            //            principalTable: "AbpUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AbpUserLogins",
            //    schema: "PHARM",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TenantId = table.Column<int>(type: "int", nullable: true),
            //        UserId = table.Column<long>(type: "bigint", nullable: false),
            //        LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //        ProviderKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AbpUserLogins", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_AbpUserLogins_AbpUsers_UserId",
            //            column: x => x.UserId,
            //            principalSchema: "dbo",
            //            principalTable: "AbpUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AbpUserOrganizationUnits",
            //    schema: "PHARM",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TenantId = table.Column<int>(type: "int", nullable: true),
            //        UserId = table.Column<long>(type: "bigint", nullable: false),
            //        OrganizationUnitId = table.Column<long>(type: "bigint", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //        CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AbpUserOrganizationUnits", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_AbpUserOrganizationUnits_AbpUsers_UserId",
            //            column: x => x.UserId,
            //            principalSchema: "dbo",
            //            principalTable: "AbpUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AbpUserRoles",
            //    schema: "PHARM",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TenantId = table.Column<int>(type: "int", nullable: true),
            //        UserId = table.Column<long>(type: "bigint", nullable: false),
            //        RoleId = table.Column<int>(type: "int", nullable: false),
            //        CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AbpUserRoles", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_AbpUserRoles_AbpUsers_UserId",
            //            column: x => x.UserId,
            //            principalSchema: "dbo",
            //            principalTable: "AbpUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AbpUserTokens",
            //    schema: "PHARM",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TenantId = table.Column<int>(type: "int", nullable: true),
            //        UserId = table.Column<long>(type: "bigint", nullable: false),
            //        LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
            //        Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
            //        Value = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
            //        ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AbpUserTokens", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_AbpUserTokens_AbpUsers_UserId",
            //            column: x => x.UserId,
            //            principalSchema: "dbo",
            //            principalTable: "AbpUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Address",
            //    schema: "ADM",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Address1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        City = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        StateId = table.Column<int>(type: "int", nullable: true),
            //        ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ContactFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ContactLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        FaxNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
            //        LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //        DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
            //        DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Address", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Address_State_StateId",
            //            column: x => x.StateId,
            //            principalSchema: "ADM",
            //            principalTable: "State",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Uom",
            //    schema: "PHARM",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TenantId = table.Column<int>(type: "int", nullable: true),
            //        UomTypeID = table.Column<int>(type: "int", nullable: true),
            //        UomName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        UomDescription = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Uom", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Uom_UomType_UomTypeID",
            //            column: x => x.UomTypeID,
            //            principalSchema: "PHARM",
            //            principalTable: "UomType",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Company",
            //    schema: "ADM",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CompanyTypeId = table.Column<int>(type: "int", nullable: true),
            //        CompanyStatusId = table.Column<int>(type: "int", nullable: true),
            //        CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        AddressId = table.Column<int>(type: "int", nullable: true),
            //        BillTo = table.Column<int>(type: "int", nullable: false),
            //        DeliveryTypeId = table.Column<int>(type: "int", nullable: false),
            //        CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
            //        LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //        DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
            //        DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Company", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Company_Address_AddressId",
            //            column: x => x.AddressId,
            //            principalSchema: "ADM",
            //            principalTable: "Address",
            //            principalColumn: "Id");
            //    });

            migrationBuilder.CreateTable(
                name: "Doctor",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignatureFileID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doctor_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Doctor_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "ADM",
                        principalTable: "Address",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Medication",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    MedicationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrandName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GenericName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Concentration = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ConcentrationUomId = table.Column<int>(type: "int", nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VolumeUomId = table.Column<int>(type: "int", nullable: false),
                    Dosage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DosageFormId = table.Column<int>(type: "int", nullable: true),
                    DosageUnits = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DosageRouteId = table.Column<int>(type: "int", nullable: true),
                    DosageFequencyId = table.Column<int>(type: "int", nullable: true),
                    DosageStrengthId = table.Column<int>(type: "int", nullable: true),
                    DispenseContainerId = table.Column<int>(type: "int", nullable: true),
                    DispenseQty = table.Column<int>(type: "int", nullable: true),
                    DispenseDoses = table.Column<int>(type: "int", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormulationTypeId = table.Column<int>(type: "int", nullable: true),
                    ExternalID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DosageFrequencyId = table.Column<int>(type: "int", nullable: false),
                    MedicationCategoryId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medication_DosageForms_DosageFormId",
                        column: x => x.DosageFormId,
                        principalSchema: "PHARM",
                        principalTable: "DosageForms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Medication_DosageFrequency_DosageFrequencyId",
                        column: x => x.DosageFrequencyId,
                        principalSchema: "PHARM",
                        principalTable: "DosageFrequency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Medication_DosageRoute_DosageRouteId",
                        column: x => x.DosageRouteId,
                        principalSchema: "PHARM",
                        principalTable: "DosageRoute",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Medication_DosageStrength_DosageStrengthId",
                        column: x => x.DosageStrengthId,
                        principalSchema: "PHARM",
                        principalTable: "DosageStrength",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Medication_FormulationType_FormulationTypeId",
                        column: x => x.FormulationTypeId,
                        principalSchema: "PHARM",
                        principalTable: "FormulationType",
                        principalColumn: "Id");
                    //table.ForeignKey(
                    //    name: "FK_Medication_Uom_ConcentrationUomId",
                    //    column: x => x.ConcentrationUomId,
                    //    principalSchema: "ADM",
                    //    principalTable: "Uom",
                    //    principalColumn: "Id",
                    //    onDelete: ReferentialAction.Cascade);
                    //table.ForeignKey(
                    //    name: "FK_Medication_Uom_VolumeUomId",
                    //    column: x => x.VolumeUomId,
                    //    principalSchema: "ADM",
                    //    principalTable: "Uom",
                    //    principalColumn: "Id",
                    //    onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateTable(
            //    name: "Facility",
            //    schema: "ADM",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        FacilityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        AddressId = table.Column<int>(type: "int", nullable: true),
            //        FacilityStatusId = table.Column<int>(type: "int", nullable: true),
            //        CompanyId = table.Column<int>(type: "int", nullable: true),
            //        CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
            //        LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //        DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
            //        DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Facility", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Facility_Address_AddressId",
            //            column: x => x.AddressId,
            //            principalSchema: "ADM",
            //            principalTable: "Address",
            //            principalColumn: "Id");
            //        table.ForeignKey(
            //            name: "FK_Facility_Company_CompanyId",
            //            column: x => x.CompanyId,
            //            principalSchema: "PHARM",
            //            principalTable: "Company",
            //            principalColumn: "Id");
            //    });

            migrationBuilder.CreateTable(
                name: "Patient",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    InsuranceID = table.Column<int>(type: "int", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GenderId = table.Column<int>(type: "int", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    EmergencyContactName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    DoctorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patient_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Patient_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "ADM",
                        principalTable: "Address",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Patient_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "PHARM",
                        principalTable: "Doctor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Patient_Insurance_InsuranceID",
                        column: x => x.InsuranceID,
                        principalSchema: "PHARM",
                        principalTable: "Insurance",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Contraindication",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    ContraindicationID = table.Column<int>(type: "int", nullable: false),
                    MedicationID = table.Column<int>(type: "int", nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contraindication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contraindication_Medication_MedicationID",
                        column: x => x.MedicationID,
                        principalSchema: "PHARM",
                        principalTable: "Medication",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationId = table.Column<int>(type: "int", nullable: true),
                    QuantityInStock = table.Column<int>(type: "int", nullable: true),
                    ReorderLevel = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_Medication_MedicationId",
                        column: x => x.MedicationId,
                        principalSchema: "PHARM",
                        principalTable: "Medication",
                        principalColumn: "Id");
                });

            //migrationBuilder.CreateTable(
            //    name: "UserCompany",
            //    schema: "ADM",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CompanyId = table.Column<int>(type: "int", nullable: false),
            //        FacilityId = table.Column<int>(type: "int", nullable: false),
            //        UserId = table.Column<long>(type: "bigint", nullable: true),
            //        UserTypeId = table.Column<int>(type: "int", nullable: true),
            //        IsDefaultFacility = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserCompany", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_UserCompany_Company_CompanyId",
            //            column: x => x.CompanyId,
            //            principalSchema: "ADM",
            //            principalTable: "Company",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_UserCompany_Facility_FacilityId",
            //            column: x => x.FacilityId,
            //            principalSchema: "ADM",
            //            principalTable: "Facility",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateTable(
                name: "PatientAllergy",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    AllergyId = table.Column<int>(type: "int", nullable: true),
                    Other = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientAllergy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientAllergy_Allergy_AllergyId",
                        column: x => x.AllergyId,
                        principalSchema: "PHARM",
                        principalTable: "Allergy",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PatientAllergy_Patient_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "PHARM",
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prescription",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescriptionStatusId = table.Column<int>(type: "int", nullable: true),
                    PrescriptionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientDateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PrescriberFacilityId = table.Column<int>(type: "int", nullable: true),
                    PrescriberName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrescriberOfficePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrescriberOfficeFax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    DoctorId = table.Column<int>(type: "int", nullable: true),
                    DosageRouteId = table.Column<int>(type: "int", nullable: true),
                    BinaryFileId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryTypeId = table.Column<int>(type: "int", nullable: true),
                    BillingTo = table.Column<int>(type: "int", nullable: true),
                    PersonFaxing = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DrugAllergies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrescriberAddress1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrescriberAddress2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrescriberNPI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrescriberSignatureFileID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PharmacyId = table.Column<int>(type: "int", nullable: true),
                    ClinicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PharmacyAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescription_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "PHARM",
                        principalTable: "Doctor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prescription_Facility_PharmacyId",
                        column: x => x.PharmacyId,
                        principalSchema: "ADM",
                        principalTable: "Facility",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prescription_Facility_PrescriberFacilityId",
                        column: x => x.PrescriberFacilityId,
                        principalSchema: "ADM",
                        principalTable: "Facility",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prescription_Patient_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "PHARM",
                        principalTable: "Patient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserCompanyDoctor",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserCompanyId = table.Column<int>(type: "int", nullable: false),
                    DoctorUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCompanyDoctor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCompanyDoctor_AbpUsers_DoctorUserId",
                        column: x => x.DoctorUserId,
                        principalSchema: "dbo",
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCompanyDoctor_UserCompany_UserCompanyId",
                        column: x => x.UserCompanyId,
                        principalSchema: "ADM",
                        principalTable: "UserCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionHistory",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    PrescriptionId = table.Column<int>(type: "int", nullable: true),
                    PreviousPrescriptionId = table.Column<int>(type: "int", nullable: true),
                    RefillHistory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousMedications = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionHistory_Prescription_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalSchema: "PHARM",
                        principalTable: "Prescription",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionItem",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    PrescriptionId = table.Column<int>(type: "int", nullable: true),
                    MedicationId = table.Column<int>(type: "int", nullable: true),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteOfAdministration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefillInformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefillsAllowed = table.Column<int>(type: "int", nullable: true),
                    RefillsRemaining = table.Column<int>(type: "int", nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionItem_Medication_MedicationId",
                        column: x => x.MedicationId,
                        principalSchema: "PHARM",
                        principalTable: "Medication",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PrescriptionItem_Prescription_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalSchema: "PHARM",
                        principalTable: "Prescription",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                schema: "PHARM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescriptionId = table.Column<int>(type: "int", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transaction_Prescription_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalSchema: "PHARM",
                        principalTable: "Prescription",
                        principalColumn: "Id");
                });

            //migrationBuilder.CreateIndex(
            //    name: "IX_AbpPermissions_UserId",
            //    schema: "PHARM",
            //    table: "AbpPermissions",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AbpSettings_UserId",
            //    schema: "PHARM",
            //    table: "AbpSettings",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AbpUserClaims_UserId",
            //    schema: "PHARM",
            //    table: "AbpUserClaims",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AbpUserLogins_UserId",
            //    schema: "PHARM",
            //    table: "AbpUserLogins",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AbpUserOrganizationUnits_UserId",
            //    schema: "PHARM",
            //    table: "AbpUserOrganizationUnits",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AbpUserRoles_UserId",
            //    schema: "PHARM",
            //    table: "AbpUserRoles",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AbpUserTokens_UserId",
            //    schema: "PHARM",
            //    table: "AbpUserTokens",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Address_StateId",
            //    schema: "ADM",
            //    table: "Address",
            //    column: "StateId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Company_AddressId",
            //    schema: "PHARM",
            //    table: "Company",
            //    column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Contraindication_MedicationID",
                schema: "PHARM",
                table: "Contraindication",
                column: "MedicationID");

            migrationBuilder.CreateIndex(
                name: "IX_Doctor_AddressId",
                schema: "PHARM",
                table: "Doctor",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctor_UserId",
                schema: "PHARM",
                table: "Doctor",
                column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Facility_AddressId",
            //    schema: "ADM",
            //    table: "Facility",
            //    column: "AddressId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Facility_CompanyId",
            //    schema: "ADM",
            //    table: "Facility",
            //    column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_MedicationId",
                schema: "PHARM",
                table: "Inventory",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_ConcentrationUomId",
                schema: "PHARM",
                table: "Medication",
                column: "ConcentrationUomId");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_DosageFormId",
                schema: "PHARM",
                table: "Medication",
                column: "DosageFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_DosageFrequencyId",
                schema: "PHARM",
                table: "Medication",
                column: "DosageFrequencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_DosageRouteId",
                schema: "PHARM",
                table: "Medication",
                column: "DosageRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_DosageStrengthId",
                schema: "PHARM",
                table: "Medication",
                column: "DosageStrengthId");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_FormulationTypeId",
                schema: "PHARM",
                table: "Medication",
                column: "FormulationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_VolumeUomId",
                schema: "PHARM",
                table: "Medication",
                column: "VolumeUomId");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_AddressId",
                schema: "PHARM",
                table: "Patient",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_DoctorId",
                schema: "PHARM",
                table: "Patient",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_InsuranceID",
                schema: "PHARM",
                table: "Patient",
                column: "InsuranceID");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_UserId",
                schema: "PHARM",
                table: "Patient",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergy_AllergyId",
                schema: "PHARM",
                table: "PatientAllergy",
                column: "AllergyId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergy_PatientId",
                schema: "PHARM",
                table: "PatientAllergy",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_DoctorId",
                schema: "PHARM",
                table: "Prescription",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_PatientId",
                schema: "PHARM",
                table: "Prescription",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_PharmacyId",
                schema: "PHARM",
                table: "Prescription",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_PrescriberFacilityId",
                schema: "PHARM",
                table: "Prescription",
                column: "PrescriberFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionHistory_PrescriptionId",
                schema: "PHARM",
                table: "PrescriptionHistory",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItem_MedicationId",
                schema: "PHARM",
                table: "PrescriptionItem",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItem_PrescriptionId",
                schema: "PHARM",
                table: "PrescriptionItem",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PrescriptionId",
                schema: "PHARM",
                table: "Transaction",
                column: "PrescriptionId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Uom_UomTypeID",
            //    schema: "PHARM",
            //    table: "Uom",
            //    column: "UomTypeID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserCompany_CompanyId",
            //    schema: "ADM",
            //    table: "UserCompany",
            //    column: "CompanyId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserCompany_FacilityId",
            //    schema: "ADM",
            //    table: "UserCompany",
            //    column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanyDoctor_DoctorUserId",
                schema: "PHARM",
                table: "UserCompanyDoctor",
                column: "DoctorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanyDoctor_UserCompanyId",
                schema: "PHARM",
                table: "UserCompanyDoctor",
                column: "UserCompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "AbpPermissions",
            //    schema: "PHARM");

            //migrationBuilder.DropTable(
            //    name: "AbpSettings",
            //    schema: "PHARM");

            //migrationBuilder.DropTable(
            //    name: "AbpUserClaims",
            //    schema: "PHARM");

            //migrationBuilder.DropTable(
            //    name: "AbpUserLogins",
            //    schema: "PHARM");

            //migrationBuilder.DropTable(
            //    name: "AbpUserOrganizationUnits",
            //    schema: "PHARM");

            //migrationBuilder.DropTable(
            //    name: "AbpUserRoles",
            //    schema: "PHARM");

            //migrationBuilder.DropTable(
            //    name: "AbpUserTokens",
            //    schema: "PHARM");

            migrationBuilder.DropTable(
                name: "Contraindication",
                schema: "PHARM");

            migrationBuilder.DropTable(
                name: "DosageDuration",
                schema: "PHARM");

            migrationBuilder.DropTable(
                name: "Inventory",
                schema: "PHARM");

            migrationBuilder.DropTable(
                name: "PatientAllergy",
                schema: "PHARM");

            migrationBuilder.DropTable(
                name: "PrescriptionHistory",
                schema: "PHARM");

            migrationBuilder.DropTable(
                name: "PrescriptionItem",
                schema: "PHARM");

            migrationBuilder.DropTable(
                name: "Transaction",
                schema: "PHARM");

            migrationBuilder.DropTable(
                name: "UserCompanyDoctor",
                schema: "PHARM");

            migrationBuilder.DropTable(
                name: "Allergy",
                schema: "PHARM");

            migrationBuilder.DropTable(
                name: "Medication",
                schema: "PHARM");

            migrationBuilder.DropTable(
                name: "Prescription",
                schema: "PHARM");

            //migrationBuilder.DropTable(
            //    name: "UserCompany",
            //    schema: "PHARM");

            migrationBuilder.DropTable(
                name: "DosageForms",
                schema: "PHARM");

            migrationBuilder.DropTable(
                name: "DosageFrequency",
                schema: "PHARM");

            migrationBuilder.DropTable(
                name: "DosageRoute",
                schema: "PHARM");

            migrationBuilder.DropTable(
                name: "DosageStrength",
                schema: "PHARM");

            migrationBuilder.DropTable(
                name: "FormulationType",
                schema: "PHARM");

            //migrationBuilder.DropTable(
            //    name: "Uom",
            //    schema: "PHARM");

            migrationBuilder.DropTable(
                name: "Patient",
                schema: "PHARM");

            //migrationBuilder.DropTable(
            //    name: "Facility",
            //    schema: "ADM");

            //migrationBuilder.DropTable(
            //    name: "UomType",
            //    schema: "PHARM");

            migrationBuilder.DropTable(
                name: "Doctor",
                schema: "PHARM");

            migrationBuilder.DropTable(
                name: "Insurance",
                schema: "PHARM");

            //migrationBuilder.DropTable(
            //    name: "Company",
            //    schema: "PHARM");

            //migrationBuilder.DropTable(
            //    name: "AbpUsers",
            //    schema: "dbo");

            //migrationBuilder.DropTable(
            //    name: "Address",
            //    schema: "ADM");

            //migrationBuilder.DropTable(
            //    name: "State",
            //    schema: "ADM");
        }
    }
}

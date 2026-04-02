using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATI.Migrations
{
    /// <inheritdoc />
    public partial class AddProcedureTypesIncrementally : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure REV schema exists
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'REV') EXEC('CREATE SCHEMA [REV]')");

            // Create ProcedureType table if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[REV].[ProcedureType]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [REV].[ProcedureType] (
                        [Id] int NOT NULL IDENTITY(1,1),
                        [Name] nvarchar(200) NOT NULL,
                        [Code] nvarchar(50) NOT NULL,
                        [CategoryGroup] int NOT NULL,
                        [Description] nvarchar(500) NOT NULL,
                        [IsActive] bit NOT NULL,
                        [DisplayOrder] int NOT NULL,
                        [CreationTime] datetime2 NOT NULL,
                        [CreatorUserId] bigint NULL,
                        [LastModificationTime] datetime2 NULL,
                        [LastModifierUserId] bigint NULL,
                        CONSTRAINT [PK_ProcedureType] PRIMARY KEY ([Id])
                    );

                    CREATE UNIQUE INDEX [IX_ProcedureType_Code] ON [REV].[ProcedureType] ([Code]);
                END
            ");

            // Create ProcedureQuota table if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[REV].[ProcedureQuota]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [REV].[ProcedureQuota] (
                        [Id] int NOT NULL IDENTITY(1,1),
                        [ProcedureTypeId] int NOT NULL,
                        [FacilityId] int NULL,
                        [QuotaPeriod] int NOT NULL,
                        [QuotaValue] decimal(18,2) NOT NULL,
                        [StartDate] datetime2 NOT NULL,
                        [EndDate] datetime2 NOT NULL,
                        [Notes] nvarchar(500) NOT NULL,
                        [CreationTime] datetime2 NOT NULL,
                        [CreatorUserId] bigint NULL,
                        [LastModificationTime] datetime2 NULL,
                        [LastModifierUserId] bigint NULL,
                        CONSTRAINT [PK_ProcedureQuota] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_ProcedureQuota_ProcedureType_ProcedureTypeId]
                            FOREIGN KEY ([ProcedureTypeId])
                            REFERENCES [REV].[ProcedureType] ([Id])
                            ON DELETE CASCADE
                    );

                    CREATE INDEX [IX_ProcedureQuota_ProcedureTypeId] ON [REV].[ProcedureQuota] ([ProcedureTypeId]);
                END
            ");

            // Add new columns to Case table if they don't exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[REV].[Case]') AND type in (N'U'))
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[REV].[Case]') AND name = 'ProcedureTypeId')
                        ALTER TABLE [REV].[Case] ADD [ProcedureTypeId] int NULL;

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[REV].[Case]') AND name = 'FacilityId')
                        ALTER TABLE [REV].[Case] ADD [FacilityId] int NULL;

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[REV].[Case]') AND name = 'SurgeonName')
                        ALTER TABLE [REV].[Case] ADD [SurgeonName] nvarchar(200) NOT NULL DEFAULT '';

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[REV].[Case]') AND name = 'ProcedureDate')
                        ALTER TABLE [REV].[Case] ADD [ProcedureDate] datetime2 NULL;
                END
            ");

            // Add foreign key constraint from Case to ProcedureType if it doesn't exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[REV].[Case]') AND type in (N'U'))
                AND EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[REV].[ProcedureType]') AND type in (N'U'))
                AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Case_ProcedureType_ProcedureTypeId')
                BEGIN
                    ALTER TABLE [REV].[Case]
                    ADD CONSTRAINT [FK_Case_ProcedureType_ProcedureTypeId]
                    FOREIGN KEY ([ProcedureTypeId])
                    REFERENCES [REV].[ProcedureType] ([Id])
                    ON DELETE SET NULL;

                    CREATE INDEX [IX_Case_ProcedureTypeId] ON [REV].[Case] ([ProcedureTypeId]);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove foreign key constraint
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Case_ProcedureType_ProcedureTypeId')
                    ALTER TABLE [REV].[Case] DROP CONSTRAINT [FK_Case_ProcedureType_ProcedureTypeId];
            ");

            // Remove index
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Case_ProcedureTypeId' AND object_id = OBJECT_ID(N'[REV].[Case]'))
                    DROP INDEX [IX_Case_ProcedureTypeId] ON [REV].[Case];
            ");

            // Remove columns from Case table
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[REV].[Case]') AND name = 'ProcedureTypeId')
                    ALTER TABLE [REV].[Case] DROP COLUMN [ProcedureTypeId];

                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[REV].[Case]') AND name = 'FacilityId')
                    ALTER TABLE [REV].[Case] DROP COLUMN [FacilityId];

                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[REV].[Case]') AND name = 'SurgeonName')
                    ALTER TABLE [REV].[Case] DROP COLUMN [SurgeonName];

                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[REV].[Case]') AND name = 'ProcedureDate')
                    ALTER TABLE [REV].[Case] DROP COLUMN [ProcedureDate];
            ");

            // Drop ProcedureQuota table
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[REV].[ProcedureQuota]') AND type in (N'U'))
                    DROP TABLE [REV].[ProcedureQuota];
            ");

            // Drop ProcedureType table
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[REV].[ProcedureType]') AND type in (N'U'))
                    DROP TABLE [REV].[ProcedureType];
            ");
        }
    }
}

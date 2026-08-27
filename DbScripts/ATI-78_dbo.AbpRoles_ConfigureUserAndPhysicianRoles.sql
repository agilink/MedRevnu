-- Set up the three roles the application is meant to have.
--
--   Admin     - sees everything. Nothing to do here: Admin is registered in
--               AppRoleConfig as a static role with grantAllPermissionsByDefault, so it
--               is granted every permission implicitly, including any added later.
--   User      - every menu except Administration.
--   Physician - the dashboard and the revenue transaction screen only, and only their
--               own hospital's data. The hospital filter is applied in code from
--               ADM.Personnel.UserId; this script only sets what they can reach.
--
-- Why this is needed: ABP grants permissions to a role only when the tenant is created
-- (TenantManager.GrantAllPermissionsAsync). Permissions defined afterwards, and roles
-- edited by hand since, are not reconciled - so on an existing database the User role
-- holds an arbitrary mix and the Physician role holds nothing at all.
--
-- Re-runnable: grants are inserted only when missing, denials are flipped rather than
-- duplicated, and the role rename only fires when it has not already happened.

SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;

------------------------------------------------------------------------------------------
-- 1. The Physician role
------------------------------------------------------------------------------------------
-- The role was created through the UI, which left Name as a GUID while DisplayName read
-- "Physician". Code looks the role up by Name, so the name has to be the real one.
UPDATE [dbo].[AbpRoles]
SET [Name] = N'Physician',
    [NormalizedName] = N'PHYSICIAN',
    [LastModificationTime] = GETUTCDATE()
WHERE [DisplayName] = N'Physician'
  AND [Name] <> N'Physician'
  AND [IsDeleted] = 0
  AND [TenantId] IS NOT NULL
  AND NOT EXISTS (
        SELECT 1 FROM [dbo].[AbpRoles] existing
        WHERE existing.[Name] = N'Physician'
          AND existing.[IsDeleted] = 0
          AND existing.[TenantId] = [dbo].[AbpRoles].[TenantId]
  );

IF @@ROWCOUNT > 0
    PRINT 'Renamed the Physician role so it can be found by name.';

-- Every tenant needs the role, including tenants that never had one created by hand.
INSERT INTO [dbo].[AbpRoles]
    ([TenantId], [Name], [NormalizedName], [DisplayName], [IsStatic], [IsDefault],
     [IsDeleted], [CreationTime], [ConcurrencyStamp])
SELECT DISTINCT
    t.[TenantId], N'Physician', N'PHYSICIAN', N'Physician', 0, 0,
    0, GETUTCDATE(), CONVERT(NVARCHAR(50), NEWID())
FROM (SELECT [TenantId] FROM [dbo].[AbpRoles] WHERE [TenantId] IS NOT NULL AND [IsDeleted] = 0) t
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[AbpRoles] r
    WHERE r.[Name] = N'Physician' AND r.[IsDeleted] = 0 AND r.[TenantId] = t.[TenantId]
);

IF @@ROWCOUNT > 0
    PRINT 'Created the Physician role where it was missing.';
GO

SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;

------------------------------------------------------------------------------------------
-- 2. What each role can reach
------------------------------------------------------------------------------------------
DECLARE @RolePermissions TABLE (RoleName NVARCHAR(64), PermissionName NVARCHAR(128),
                                PRIMARY KEY (RoleName, PermissionName));

-- User: everything except Administration. Host-only permissions (Editions, Tenants) are
-- left out - a tenant role cannot act on them.
INSERT INTO @RolePermissions (RoleName, PermissionName) VALUES
    (N'User', N'Pages'),
    (N'User', N'Pages.Tenant.Dashboard'),
    (N'User', N'Pages.Revenue'),
    (N'User', N'Pages.Revenue.Dashboard'),
    (N'User', N'Pages.Revenue.Reports'),
    (N'User', N'Pages.Revenue.ProcedureTransactions'),
    (N'User', N'Pages.Revenue.ProcedureTransactions.Create'),
    (N'User', N'Pages.Revenue.ProcedureTransactions.Edit'),
    (N'User', N'Pages.Revenue.ProcedureTransactions.Delete'),
    (N'User', N'Pages.Revenue.Physicians'),
    (N'User', N'Pages.Revenue.Physicians.Create'),
    (N'User', N'Pages.Revenue.Physicians.Edit'),
    (N'User', N'Pages.Revenue.Physicians.Delete'),
    (N'User', N'Pages.Revenue.Hospitals'),
    (N'User', N'Pages.Revenue.Hospitals.Create'),
    (N'User', N'Pages.Revenue.Hospitals.Edit'),
    (N'User', N'Pages.Revenue.Hospitals.Delete'),
    (N'User', N'Pages.Revenue.Products'),
    (N'User', N'Pages.Revenue.Products.Create'),
    (N'User', N'Pages.Revenue.Products.Edit'),
    (N'User', N'Pages.Revenue.Products.Delete'),
    (N'User', N'Pages.Revenue.ProductQuotas'),
    (N'User', N'Pages.Revenue.ProductQuotas.Create'),
    (N'User', N'Pages.Revenue.ProductQuotas.Edit'),
    (N'User', N'Pages.Revenue.ProductQuotas.Delete'),
    (N'User', N'Pages.Revenue.HospitalProductPrices'),
    (N'User', N'Pages.Revenue.HospitalProductPrices.Create'),
    (N'User', N'Pages.Revenue.HospitalProductPrices.Edit'),
    (N'User', N'Pages.Revenue.HospitalProductPrices.Delete');

-- Physician: the dashboard and the transaction screen, with full rights on transactions
-- for their own hospital. Pages.Revenue is deliberately withheld - the Configuration menu
-- hangs off it, so granting it would put a menu of reference-data screens in front of a
-- physician.
INSERT INTO @RolePermissions (RoleName, PermissionName) VALUES
    (N'Physician', N'Pages'),
    (N'Physician', N'Pages.Revenue.Dashboard'),
    (N'Physician', N'Pages.Revenue.ProcedureTransactions'),
    (N'Physician', N'Pages.Revenue.ProcedureTransactions.Create'),
    (N'Physician', N'Pages.Revenue.ProcedureTransactions.Edit'),
    (N'Physician', N'Pages.Revenue.ProcedureTransactions.Delete');

-- Grant what is missing.
INSERT INTO [dbo].[AbpPermissions]
    ([CreationTime], [CreatorUserId], [Name], [IsGranted], [TenantId], [Discriminator], [RoleId])
SELECT
    GETUTCDATE(), NULL, rp.PermissionName, 1, r.[TenantId], N'RolePermissionSetting', r.[Id]
FROM [dbo].[AbpRoles] r
INNER JOIN @RolePermissions rp ON rp.RoleName = r.[Name]
WHERE r.[IsDeleted] = 0
  AND r.[TenantId] IS NOT NULL
  AND NOT EXISTS (
        SELECT 1 FROM [dbo].[AbpPermissions] ap
        WHERE ap.[Discriminator] = N'RolePermissionSetting'
          AND ap.[RoleId] = r.[Id]
          AND ap.[Name] = rp.PermissionName
  );

PRINT 'Granted the User and Physician role permissions.';

-- Flip any explicit denial on a permission the role is supposed to have.
UPDATE ap
SET ap.[IsGranted] = 1
FROM [dbo].[AbpPermissions] ap
INNER JOIN [dbo].[AbpRoles] r ON r.[Id] = ap.[RoleId]
INNER JOIN @RolePermissions rp ON rp.RoleName = r.[Name] AND rp.PermissionName = ap.[Name]
WHERE ap.[Discriminator] = N'RolePermissionSetting'
  AND r.[IsDeleted] = 0
  AND ap.[IsGranted] = 0;
GO

SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;

------------------------------------------------------------------------------------------
-- 3. Take away what these roles must not have
------------------------------------------------------------------------------------------
-- The User role had been given Pages.Administration and most of Users.* by hand, which is
-- exactly what "every menu except Administration" rules out.
DELETE ap
FROM [dbo].[AbpPermissions] ap
INNER JOIN [dbo].[AbpRoles] r ON r.[Id] = ap.[RoleId]
WHERE ap.[Discriminator] = N'RolePermissionSetting'
  AND r.[IsDeleted] = 0
  AND r.[Name] = N'User'
  AND ap.[Name] LIKE N'Pages.Administration%';

IF @@ROWCOUNT > 0
    PRINT 'Removed Administration access from the User role.';

-- A physician must not reach anything beyond the two screens listed above. Anything else
-- attached to the role - by an earlier run of a different list, or by hand - is removed.
DELETE ap
FROM [dbo].[AbpPermissions] ap
INNER JOIN [dbo].[AbpRoles] r ON r.[Id] = ap.[RoleId]
WHERE ap.[Discriminator] = N'RolePermissionSetting'
  AND r.[IsDeleted] = 0
  AND r.[Name] = N'Physician'
  AND ap.[Name] NOT IN (
        N'Pages',
        N'Pages.Revenue.Dashboard',
        N'Pages.Revenue.ProcedureTransactions',
        N'Pages.Revenue.ProcedureTransactions.Create',
        N'Pages.Revenue.ProcedureTransactions.Edit',
        N'Pages.Revenue.ProcedureTransactions.Delete'
  );

IF @@ROWCOUNT > 0
    PRINT 'Removed permissions the Physician role should not hold.';

-- Pages_Medrev is a leftover permission that no menu item or controller checks.
DELETE FROM [dbo].[AbpPermissions] WHERE [Name] = N'Pages_Medrev';

IF @@ROWCOUNT > 0
    PRINT 'Removed grants for the unused Pages_Medrev permission.';
GO

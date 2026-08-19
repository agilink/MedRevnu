-- Grant the new Pages.Revenue.* permissions to every Admin role.
--
-- Why this is needed: ABP only calls GrantAllPermissionsAsync when a tenant is
-- first created (see TenantManager.cs). Permissions added afterwards are not
-- granted to existing roles, so without this script the new Revenue menu items
-- (Revenue Transactions, Reports, Dashboard, Products, Product Quotas) would be
-- defined but invisible on any database that already exists.
--
-- Re-runnable: each grant is inserted only if it is not already present.

SET NOCOUNT ON;

DECLARE @RevenuePermissions TABLE (Name NVARCHAR(128) PRIMARY KEY);

INSERT INTO @RevenuePermissions (Name) VALUES
    ('Pages.Revenue'),
    ('Pages.Revenue.Dashboard'),
    ('Pages.Revenue.Reports'),
    ('Pages.Revenue.HospitalProductPrices'),
    ('Pages.Revenue.HospitalProductPrices.Create'),
    ('Pages.Revenue.HospitalProductPrices.Edit'),
    ('Pages.Revenue.HospitalProductPrices.Delete'),
    ('Pages.Revenue.ProcedureTransactions'),
    ('Pages.Revenue.ProcedureTransactions.Create'),
    ('Pages.Revenue.ProcedureTransactions.Edit'),
    ('Pages.Revenue.ProcedureTransactions.Delete'),
    ('Pages.Revenue.ProductQuotas'),
    ('Pages.Revenue.ProductQuotas.Create'),
    ('Pages.Revenue.ProductQuotas.Edit'),
    ('Pages.Revenue.ProductQuotas.Delete'),
    ('Pages.Revenue.Products'),
    ('Pages.Revenue.Products.Create'),
    ('Pages.Revenue.Products.Edit'),
    ('Pages.Revenue.Products.Delete'),
    ('Pages.Revenue.Cases'),
    ('Pages.Revenue.Cases.Create'),
    ('Pages.Revenue.Cases.Edit'),
    ('Pages.Revenue.Cases.Delete');

-- Insert a granted RolePermissionSetting for every Admin role that is missing it.
INSERT INTO [dbo].[AbpPermissions]
    ([CreationTime], [CreatorUserId], [Name], [IsGranted], [TenantId], [Discriminator], [RoleId])
SELECT
    GETUTCDATE(), NULL, p.Name, 1, r.TenantId, N'RolePermissionSetting', r.Id
FROM [dbo].[AbpRoles] r
CROSS JOIN @RevenuePermissions p
WHERE r.Name = N'Admin'
  AND r.IsDeleted = 0
  AND NOT EXISTS (
        SELECT 1
        FROM [dbo].[AbpPermissions] ap
        WHERE ap.[Discriminator] = N'RolePermissionSetting'
          AND ap.[RoleId] = r.Id
          AND ap.[Name] = p.Name
  );

-- Flip any pre-existing explicit denials on these permissions for Admin roles.
UPDATE ap
SET ap.[IsGranted] = 1
FROM [dbo].[AbpPermissions] ap
INNER JOIN [dbo].[AbpRoles] r ON r.Id = ap.[RoleId]
INNER JOIN @RevenuePermissions p ON p.Name = ap.[Name]
WHERE ap.[Discriminator] = N'RolePermissionSetting'
  AND r.Name = N'Admin'
  AND r.IsDeleted = 0
  AND ap.[IsGranted] = 0;

PRINT 'Revenue permissions granted to Admin roles.';

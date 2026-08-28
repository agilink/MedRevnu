-- Give the last six products a subcategory, and add a home for leads and accessories.
--
-- Why this is needed: the device dropdown on a case now offers only products whose
-- subcategory matches the case's implant type. A product with no subcategory cannot be
-- judged, so it is offered on every case regardless - six products were in that state and
-- were the one remaining hole in the rule.
--
-- Two separate problems were found:
--
--  1. Five products sat in two categories, "Pacemaker Devices" and "Defibrillator
--     Devices", that duplicate the real taxonomy and had no subcategories at all. They
--     are moved into the real categories and given subcategories. Those two categories
--     are then empty and are removed.
--
--  2. Ultipace was in the real High Voltage category but priced at $1,478-$1,568 across
--     five hospitals - lead pricing, an order of magnitude below every generator. There
--     was nowhere for a lead to go, so a "Leads & Accessories" category and subcategory
--     are added.
--
-- The leads subcategory has a NULL ImplantType, meaning "belongs on either kind of
-- case". A lead is fitted during both new implants and generator changes, so forcing it
-- to pick one would make it unusable on half of them. Run
-- ATI-78_REV.ProductSubcategory_MakeImplantTypeNullable.sql first.
--
-- Re-runnable: every insert is guarded on absence and every update is guarded on the
-- product still being unmapped, so a second run changes nothing.

SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;

------------------------------------------------------------------------------------------
-- 1. A category and subcategory for leads and accessories
------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [REV].[ProductCategory] WHERE [Name] = N'Leads & Accessories')
BEGIN
    INSERT INTO [REV].[ProductCategory] ([Name], [CreationTime])
    VALUES (N'Leads & Accessories', GETUTCDATE());

    PRINT 'Created the Leads & Accessories category.';
END

DECLARE @LeadsCategoryId INT =
    (SELECT TOP 1 [Id] FROM [REV].[ProductCategory] WHERE [Name] = N'Leads & Accessories');

IF NOT EXISTS (
    SELECT 1 FROM [REV].[ProductSubcategory]
    WHERE [SubcategoryName] = N'Leads & Accessories' AND [ProductCategoryId] = @LeadsCategoryId
)
BEGIN
    -- NULL implant type: fitted during both new implants and generator changes.
    INSERT INTO [REV].[ProductSubcategory]
        ([ProductCategoryId], [SubcategoryName], [Description], [ImplantType], [CreationTime])
    VALUES
        (@LeadsCategoryId, N'Leads & Accessories',
         N'Leads and accessories - valid on both new implants and generator changes',
         NULL, GETUTCDATE());

    PRINT 'Created the Leads & Accessories subcategory.';
END
GO

SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;

------------------------------------------------------------------------------------------
-- 2. Map the six unmapped products
------------------------------------------------------------------------------------------
-- Each product is matched by its code (or name, where it has no code) and mapped to a
-- subcategory by name, so the script does not depend on identity values.
DECLARE @Mapping TABLE (
    MatchCode       NVARCHAR(100) NULL,
    MatchName       NVARCHAR(200) NULL,
    SubcategoryName NVARCHAR(50)  NOT NULL,
    Rationale       NVARCHAR(200) NOT NULL
);

INSERT INTO @Mapping (MatchCode, MatchName, SubcategoryName, Rationale) VALUES
    -- Leadless ventricular pacemaker with atrial sensing.
    (N'PM-001', NULL, N'VR',                  N'Micra AV is a leadless ventricular pacemaker'),
    -- Gallant HF is Abbott's CRT-D; it was filed under pacemakers.
    (N'PM-002', NULL, N'CRT-D',               N'Gallant HF is a CRT-D, not a pacemaker'),
    -- EMBLEM MRI is a subcutaneous single chamber ICD.
    (N'ICD-001', NULL, N'Single Chamber ICD', N'EMBLEM MRI is a single chamber S-ICD'),
    (N'ICD-002', NULL, N'CRT-D',              N'Named as a CRT-D'),
    (N'LEAD-001', NULL, N'Leads & Accessories', N'A pacing lead'),
    -- Ultipace has no product code, so it is matched by name.
    (NULL, N'Ultipace', N'Leads & Accessories', N'Priced with the leads, far below any generator');

UPDATE p
SET p.[SubproductCategoryId] = s.[Id],
    -- Keep the product's category consistent with the subcategory it now sits under,
    -- otherwise the two disagree and reports group it by the wrong device type.
    p.[ProductCategoryID] = s.[ProductCategoryId],
    p.[LastModificationTime] = GETUTCDATE()
FROM [REV].[Product] p
INNER JOIN @Mapping m
    ON (m.MatchCode IS NOT NULL AND LTRIM(RTRIM(REPLACE(REPLACE(p.[ProductCode], CHAR(13), ''), CHAR(10), ''))) = m.MatchCode)
    OR (m.MatchName IS NOT NULL AND LTRIM(RTRIM(REPLACE(REPLACE(p.[Name], CHAR(13), ''), CHAR(10), ''))) = m.MatchName)
INNER JOIN [REV].[ProductSubcategory] s ON s.[SubcategoryName] = m.SubcategoryName
WHERE p.[SubproductCategoryId] IS NULL;

PRINT CONCAT('Mapped ', @@ROWCOUNT, ' product(s) to a subcategory.');
GO

SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;

------------------------------------------------------------------------------------------
-- 3. Remove the duplicate categories, now that nothing is left in them
------------------------------------------------------------------------------------------
-- Guarded on being genuinely empty, so the script can never delete a category someone has
-- since put a product, subcategory or quota against.
DELETE c
FROM [REV].[ProductCategory] c
WHERE c.[Name] IN (N'Pacemaker Devices', N'Defibrillator Devices')
  AND NOT EXISTS (SELECT 1 FROM [REV].[Product] p WHERE p.[ProductCategoryID] = c.[Id])
  AND NOT EXISTS (SELECT 1 FROM [REV].[ProductSubcategory] s WHERE s.[ProductCategoryId] = c.[Id])
  AND NOT EXISTS (SELECT 1 FROM [REV].[ProductQuota] q WHERE q.[ProductCategoryId] = c.[Id]);

IF @@ROWCOUNT > 0
    PRINT 'Removed the empty duplicate categories.';
GO

SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;

------------------------------------------------------------------------------------------
-- 4. Report anything still unmapped
------------------------------------------------------------------------------------------
IF EXISTS (SELECT 1 FROM [REV].[Product] WHERE [SubproductCategoryId] IS NULL)
BEGIN
    PRINT 'STILL UNMAPPED - these are offered on every case regardless of implant type:';
    SELECT [Id], [ProductCode], [Name], [ProductCategoryID]
    FROM [REV].[Product] WHERE [SubproductCategoryId] IS NULL ORDER BY [Id];
END
ELSE
BEGIN
    PRINT 'Every product now has a subcategory.';
END
GO

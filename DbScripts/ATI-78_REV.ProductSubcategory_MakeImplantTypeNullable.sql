-- Let a subcategory decline to pick an implant type.
--
-- Why this is needed: every subcategory had to be either De Novo or Gen Change, and the
-- device dropdown on a case now uses that to decide what to offer. Leads and accessories
-- are fitted during both new implants and generator changes, so under the old rule a lead
-- would have had to pick one and become unusable on half the cases.
--
-- NULL means "no restriction". The dropdown offers such a product on either kind of case,
-- and the save-time check treats it as never mismatched.
--
-- No existing row is changed: all 23 subcategories keep the implant type they have.
--
-- Re-runnable: the alter is skipped once the column is already nullable.

SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;

IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[REV].[ProductSubcategory]')
      AND name = 'ImplantType'
      AND is_nullable = 0
)
BEGIN
    ALTER TABLE [REV].[ProductSubcategory] ALTER COLUMN [ImplantType] INT NULL;
    PRINT 'REV.ProductSubcategory.ImplantType is now nullable.';
END
ELSE
BEGIN
    PRINT 'REV.ProductSubcategory.ImplantType is already nullable.';
END
GO

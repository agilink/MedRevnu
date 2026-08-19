-- ATI-78: Make optional string columns nullable in REV.Case, REV.Product, REV.ProductCategory
-- Re-runnable: ALTER COLUMN is idempotent when column is already nullable

-- REV.Case
ALTER TABLE [REV].[Case] ALTER COLUMN [Description] NVARCHAR(500) NULL;
ALTER TABLE [REV].[Case] ALTER COLUMN [Notes] NVARCHAR(1000) NULL;
ALTER TABLE [REV].[Case] ALTER COLUMN [SurgeonName] NVARCHAR(200) NULL;

-- REV.Product
ALTER TABLE [REV].[Product] ALTER COLUMN [ProductCode] NVARCHAR(100) NULL;
ALTER TABLE [REV].[Product] ALTER COLUMN [Manufacturer] NVARCHAR(200) NULL;
ALTER TABLE [REV].[Product] ALTER COLUMN [ModelNo] NVARCHAR(100) NULL;
ALTER TABLE [REV].[Product] ALTER COLUMN [Description] NVARCHAR(500) NULL;

-- REV.ProductCategory
ALTER TABLE [REV].[ProductCategory] ALTER COLUMN [ShortDescription] NVARCHAR(500) NULL;
ALTER TABLE [REV].[ProductCategory] ALTER COLUMN [Description] NVARCHAR(500) NULL;

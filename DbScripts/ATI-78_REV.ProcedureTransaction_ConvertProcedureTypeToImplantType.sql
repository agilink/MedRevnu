-- Replace the free-text DE_NOVO / GEN_CHANGE strings with the ImplantType enum.
--
-- Why: De Novo vs Gen Change drives case counts, revenue splits and quota targets,
-- and pricing differs between them. It was stored as nvarchar(20) on two tables and
-- compared with string literals in the reports, so a typo or casing difference would
-- silently drop rows out of a report rather than fail.
--
-- ImplantType values: 1 = DeNovo, 2 = GenChange
--
-- Re-runnable: each step is guarded, so running twice is a no-op. The conversion is
-- data-preserving - the new column is populated from the old one before it is dropped.

SET NOCOUNT ON;

/* ---------------------------------------------------------------------------
   REV.ProcedureTransaction
   --------------------------------------------------------------------------- */

IF NOT EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]')
                 AND name = 'ImplantType')
BEGIN
    ALTER TABLE [REV].[ProcedureTransaction] ADD [ImplantType] INT NULL;
    PRINT 'REV.ProcedureTransaction: added ImplantType.';
END
GO

IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]')
             AND name = 'ProcedureType')
BEGIN
    DECLARE @Unmapped INT;

    SELECT @Unmapped = COUNT(*)
    FROM [REV].[ProcedureTransaction]
    WHERE [ImplantType] IS NULL
      AND UPPER(LTRIM(RTRIM(ISNULL([ProcedureType], '')))) NOT IN ('DE_NOVO', 'GEN_CHANGE');

    IF @Unmapped > 0
        PRINT 'REV.ProcedureTransaction: ' + CAST(@Unmapped AS VARCHAR(20))
            + ' row(s) had an unrecognised ProcedureType and default to DeNovo.';

    UPDATE [REV].[ProcedureTransaction]
    SET [ImplantType] = CASE UPPER(LTRIM(RTRIM(ISNULL([ProcedureType], ''))))
                            WHEN 'GEN_CHANGE' THEN 2
                            ELSE 1
                        END
    WHERE [ImplantType] IS NULL;

    PRINT 'REV.ProcedureTransaction: populated ImplantType from ProcedureType.';
END
ELSE
BEGIN
    -- Column already dropped by a previous run; make sure nothing was left behind.
    UPDATE [REV].[ProcedureTransaction] SET [ImplantType] = 1 WHERE [ImplantType] IS NULL;
END
GO

IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]')
             AND name = 'ImplantType' AND is_nullable = 1)
BEGIN
    ALTER TABLE [REV].[ProcedureTransaction] ALTER COLUMN [ImplantType] INT NOT NULL;
    PRINT 'REV.ProcedureTransaction: ImplantType set to NOT NULL.';
END
GO

IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]')
             AND name = 'ProcedureType')
BEGIN
    ALTER TABLE [REV].[ProcedureTransaction] DROP COLUMN [ProcedureType];
    PRINT 'REV.ProcedureTransaction: dropped ProcedureType.';
END
GO

/* ---------------------------------------------------------------------------
   REV.ProductSubcategory
   --------------------------------------------------------------------------- */

IF NOT EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID(N'[REV].[ProductSubcategory]')
                 AND name = 'ImplantType')
BEGIN
    ALTER TABLE [REV].[ProductSubcategory] ADD [ImplantType] INT NULL;
    PRINT 'REV.ProductSubcategory: added ImplantType.';
END
GO

IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N'[REV].[ProductSubcategory]')
             AND name = 'ProcedureType')
BEGIN
    DECLARE @UnmappedSub INT;

    SELECT @UnmappedSub = COUNT(*)
    FROM [REV].[ProductSubcategory]
    WHERE [ImplantType] IS NULL
      AND UPPER(LTRIM(RTRIM(ISNULL([ProcedureType], '')))) NOT IN ('DE_NOVO', 'GEN_CHANGE');

    IF @UnmappedSub > 0
        PRINT 'REV.ProductSubcategory: ' + CAST(@UnmappedSub AS VARCHAR(20))
            + ' row(s) had an unrecognised ProcedureType and default to DeNovo.';

    UPDATE [REV].[ProductSubcategory]
    SET [ImplantType] = CASE UPPER(LTRIM(RTRIM(ISNULL([ProcedureType], ''))))
                            WHEN 'GEN_CHANGE' THEN 2
                            ELSE 1
                        END
    WHERE [ImplantType] IS NULL;

    PRINT 'REV.ProductSubcategory: populated ImplantType from ProcedureType.';
END
ELSE
BEGIN
    UPDATE [REV].[ProductSubcategory] SET [ImplantType] = 1 WHERE [ImplantType] IS NULL;
END
GO

IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N'[REV].[ProductSubcategory]')
             AND name = 'ImplantType' AND is_nullable = 1)
BEGIN
    ALTER TABLE [REV].[ProductSubcategory] ALTER COLUMN [ImplantType] INT NOT NULL;
    PRINT 'REV.ProductSubcategory: ImplantType set to NOT NULL.';
END
GO

IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N'[REV].[ProductSubcategory]')
             AND name = 'ProcedureType')
BEGIN
    ALTER TABLE [REV].[ProductSubcategory] DROP COLUMN [ProcedureType];
    PRINT 'REV.ProductSubcategory: dropped ProcedureType.';
END
GO

SELECT
    'ProcedureTransaction' AS TableName,
    [ImplantType],
    COUNT(*) AS RowCountByType
FROM [REV].[ProcedureTransaction]
GROUP BY [ImplantType]
UNION ALL
SELECT
    'ProductSubcategory',
    [ImplantType],
    COUNT(*)
FROM [REV].[ProductSubcategory]
GROUP BY [ImplantType]
ORDER BY TableName, [ImplantType];
GO

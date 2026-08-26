-- Replace the free-text DE_NOVO / GEN_CHANGE strings with the ImplantType enum.
--
-- Why: De Novo vs Gen Change drives case counts, revenue splits and quota targets,
-- and pricing differs between them. It was stored as nvarchar(20) on two tables and
-- compared with string literals in the reports, so a typo or casing difference would
-- silently drop rows out of a report rather than fail.
--
-- ImplantType values: 1 = DeNovo, 2 = GenChange
--
-- Re-runnable: every step is guarded, so a second run is a no-op. The statements that
-- read the old column go through sp_executesql because SQL Server compiles a whole
-- batch up front - a guarded reference to a column that has already been dropped is
-- still a compile-time error, so the guard alone is not enough.

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

SET NOCOUNT ON;

DECLARE @Tables TABLE (TableName SYSNAME PRIMARY KEY);
INSERT INTO @Tables (TableName) VALUES ('ProcedureTransaction'), ('ProductSubcategory');

DECLARE @TableName SYSNAME, @Sql NVARCHAR(MAX), @Unmapped INT;

DECLARE TableCursor CURSOR LOCAL FAST_FORWARD FOR SELECT TableName FROM @Tables;
OPEN TableCursor;
FETCH NEXT FROM TableCursor INTO @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF OBJECT_ID(N'[REV].[' + @TableName + N']') IS NULL
    BEGIN
        PRINT 'REV.' + @TableName + ' does not exist - skipped.';
        FETCH NEXT FROM TableCursor INTO @TableName;
        CONTINUE;
    END

    -- 1. Add the enum column.
    IF NOT EXISTS (SELECT 1 FROM sys.columns
                   WHERE object_id = OBJECT_ID(N'[REV].[' + @TableName + N']')
                     AND name = 'ImplantType')
    BEGIN
        SET @Sql = N'ALTER TABLE [REV].' + QUOTENAME(@TableName) + N' ADD [ImplantType] INT NULL;';
        EXEC sp_executesql @Sql;
        PRINT 'REV.' + @TableName + ': added ImplantType.';
    END

    -- 2. Carry the old string values across.
    IF EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID(N'[REV].[' + @TableName + N']')
                 AND name = 'ProcedureType')
    BEGIN
        SET @Sql = N'
            SELECT @UnmappedOut = COUNT(*)
            FROM [REV].' + QUOTENAME(@TableName) + N'
            WHERE [ImplantType] IS NULL
              AND UPPER(LTRIM(RTRIM(ISNULL([ProcedureType], '''')))) NOT IN (''DE_NOVO'', ''GEN_CHANGE'');';

        EXEC sp_executesql @Sql, N'@UnmappedOut INT OUTPUT', @UnmappedOut = @Unmapped OUTPUT;

        IF @Unmapped > 0
            PRINT 'REV.' + @TableName + ': ' + CAST(@Unmapped AS VARCHAR(20))
                + ' row(s) had an unrecognised ProcedureType and default to DeNovo.';

        SET @Sql = N'
            UPDATE [REV].' + QUOTENAME(@TableName) + N'
            SET [ImplantType] = CASE UPPER(LTRIM(RTRIM(ISNULL([ProcedureType], ''''))))
                                    WHEN ''GEN_CHANGE'' THEN 2
                                    ELSE 1
                                END
            WHERE [ImplantType] IS NULL;';

        EXEC sp_executesql @Sql;
        PRINT 'REV.' + @TableName + ': populated ImplantType from ProcedureType.';
    END
    ELSE
    BEGIN
        -- Old column already gone; make sure no row was left without a value.
        SET @Sql = N'UPDATE [REV].' + QUOTENAME(@TableName)
                 + N' SET [ImplantType] = 1 WHERE [ImplantType] IS NULL;';
        EXEC sp_executesql @Sql;
    END

    -- 3. Enforce NOT NULL.
    IF EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID(N'[REV].[' + @TableName + N']')
                 AND name = 'ImplantType' AND is_nullable = 1)
    BEGIN
        SET @Sql = N'ALTER TABLE [REV].' + QUOTENAME(@TableName)
                 + N' ALTER COLUMN [ImplantType] INT NOT NULL;';
        EXEC sp_executesql @Sql;
        PRINT 'REV.' + @TableName + ': ImplantType set to NOT NULL.';
    END

    -- 4. Drop the old column.
    IF EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID(N'[REV].[' + @TableName + N']')
                 AND name = 'ProcedureType')
    BEGIN
        SET @Sql = N'ALTER TABLE [REV].' + QUOTENAME(@TableName)
                 + N' DROP COLUMN [ProcedureType];';
        EXEC sp_executesql @Sql;
        PRINT 'REV.' + @TableName + ': dropped ProcedureType.';
    END

    FETCH NEXT FROM TableCursor INTO @TableName;
END

CLOSE TableCursor;
DEALLOCATE TableCursor;

SELECT
    'ProcedureTransaction' AS TableName,
    [ImplantType],
    COUNT(*) AS RowsByType
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

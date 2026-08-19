-- Make optional REV text columns nullable to match the entity definitions.
--
-- Why: these properties are declared as `string?` in the domain entities, but the
-- model snapshot had never caught up, so the tables were still created NOT NULL.
-- Saving a record without, say, a Manufacturer or Notes value therefore failed at
-- the database even though the application treats the field as optional.
--
-- This ships alongside the ConvertProcedureTypeToImplantType migration, which
-- carries the same relaxations.
--
-- Re-runnable: each column is altered only while it is still NOT NULL.

SET NOCOUNT ON;

DECLARE @Columns TABLE (
    TableName  SYSNAME,
    ColumnName SYSNAME,
    DataType   NVARCHAR(50)
);

INSERT INTO @Columns (TableName, ColumnName, DataType) VALUES
    ('ProductSubcategory',   'Description',      'nvarchar(500)'),
    ('ProductCategory',      'ShortDescription', 'nvarchar(50)'),
    ('ProductCategory',      'Description',      'nvarchar(500)'),
    ('Product',              'ProductCode',      'nvarchar(30)'),
    ('Product',              'ModelNo',          'nvarchar(100)'),
    ('Product',              'Manufacturer',     'nvarchar(200)'),
    ('Product',              'Description',      'nvarchar(500)'),
    ('ProcedureType',        'Description',      'nvarchar(500)'),
    ('ProcedureQuota',       'Notes',            'nvarchar(500)'),
    ('HospitalProductPrice', 'ProductCode',      'nvarchar(100)'),
    ('Case',                 'SurgeonName',      'nvarchar(200)'),
    ('Case',                 'Notes',            'nvarchar(1000)'),
    ('Case',                 'Description',      'nvarchar(500)');

DECLARE @TableName SYSNAME, @ColumnName SYSNAME, @DataType NVARCHAR(50), @Sql NVARCHAR(MAX);
DECLARE @IndexName SYSNAME, @IndexIsUnique BIT, @Blocked INT;

DECLARE ColumnCursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT c.TableName, c.ColumnName, c.DataType
    FROM @Columns c
    INNER JOIN sys.columns sc
            ON sc.object_id = OBJECT_ID(N'[REV].[' + c.TableName + N']')
           AND sc.name = c.ColumnName
    WHERE sc.is_nullable = 0;

OPEN ColumnCursor;
FETCH NEXT FROM ColumnCursor INTO @TableName, @ColumnName, @DataType;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- SQL Server refuses ALTER COLUMN while an index depends on the column, so a
    -- simple single-column index is dropped and recreated around the change.
    SET @IndexName = NULL;
    SET @IndexIsUnique = 0;

    SELECT TOP 1 @IndexName = i.name, @IndexIsUnique = i.is_unique
    FROM sys.indexes i
    JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
    JOIN sys.columns c        ON c.object_id  = ic.object_id AND c.column_id = ic.column_id
    WHERE i.object_id = OBJECT_ID(N'[REV].[' + @TableName + N']')
      AND c.name = @ColumnName
      AND i.is_primary_key = 0
      AND i.is_unique_constraint = 0
      AND i.has_filter = 0
      AND (SELECT COUNT(*) FROM sys.index_columns ic2
           WHERE ic2.object_id = i.object_id AND ic2.index_id = i.index_id) = 1;

    -- Anything more involved than that is left alone rather than guessed at.
    SELECT @Blocked = COUNT(*)
    FROM sys.indexes i
    JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
    JOIN sys.columns c        ON c.object_id  = ic.object_id AND c.column_id = ic.column_id
    WHERE i.object_id = OBJECT_ID(N'[REV].[' + @TableName + N']')
      AND c.name = @ColumnName
      AND (i.is_primary_key = 1 OR i.is_unique_constraint = 1 OR i.has_filter = 1
           OR (SELECT COUNT(*) FROM sys.index_columns ic2
               WHERE ic2.object_id = i.object_id AND ic2.index_id = i.index_id) > 1);

    IF @Blocked > 0
    BEGIN
        PRINT 'SKIPPED REV.' + @TableName + '.' + @ColumnName
            + ' - a key, unique constraint, filtered or composite index depends on it. Resolve manually.';
    END
    ELSE
    BEGIN
        IF @IndexName IS NOT NULL
        BEGIN
            SET @Sql = N'DROP INDEX ' + QUOTENAME(@IndexName)
                     + N' ON [REV].' + QUOTENAME(@TableName) + N';';
            EXEC sp_executesql @Sql;
        END

        SET @Sql = N'ALTER TABLE [REV].' + QUOTENAME(@TableName)
                 + N' ALTER COLUMN ' + QUOTENAME(@ColumnName) + N' ' + @DataType + N' NULL;';
        EXEC sp_executesql @Sql;

        IF @IndexName IS NOT NULL
        BEGIN
            SET @Sql = N'CREATE ' + CASE WHEN @IndexIsUnique = 1 THEN N'UNIQUE ' ELSE N'' END
                     + N'INDEX ' + QUOTENAME(@IndexName)
                     + N' ON [REV].' + QUOTENAME(@TableName)
                     + N' (' + QUOTENAME(@ColumnName) + N');';
            EXEC sp_executesql @Sql;
            PRINT 'REV.' + @TableName + '.' + @ColumnName
                + ' is now nullable (recreated index ' + @IndexName + ').';
        END
        ELSE
        BEGIN
            PRINT 'REV.' + @TableName + '.' + @ColumnName + ' is now nullable.';
        END
    END

    FETCH NEXT FROM ColumnCursor INTO @TableName, @ColumnName, @DataType;
END

CLOSE ColumnCursor;
DEALLOCATE ColumnCursor;

PRINT 'Optional REV text columns checked.';

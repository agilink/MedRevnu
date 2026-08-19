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
    SET @Sql = N'ALTER TABLE [REV].' + QUOTENAME(@TableName)
             + N' ALTER COLUMN ' + QUOTENAME(@ColumnName) + N' ' + @DataType + N' NULL;';

    EXEC sp_executesql @Sql;
    PRINT 'REV.' + @TableName + '.' + @ColumnName + ' is now nullable.';

    FETCH NEXT FROM ColumnCursor INTO @TableName, @ColumnName, @DataType;
END

CLOSE ColumnCursor;
DEALLOCATE ColumnCursor;

PRINT 'Optional REV text columns checked.';

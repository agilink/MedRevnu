-- Move the device off the case and onto lines, so a case can use two or three devices.
--
-- Why: a procedure often involves more than one device (a generator plus a lead, for
-- example). REV.ProcedureTransaction held a single ProductId, Quantity and UnitPrice, so
-- a multi-device case could not be recorded at all. Its Quantity column also doubled as
-- both a device count and a case count, which every case-count report summed - so the
-- moment multi-device cases existed, those reports would have over-counted.
--
-- After this: one row in REV.ProcedureTransaction is one case, identified by CaseNumber,
-- and REV.ProcedureTransactionProduct holds a row per device. Case counts come from the
-- case rows; anything per product or per category comes from the lines.
--
-- Data-preserving and re-runnable: the existing single device on each case is copied to a
-- line before the old columns are dropped, and every step is guarded.

SET NOCOUNT ON;

/* 1. Case number ---------------------------------------------------------------- */

IF NOT EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]') AND name = 'CaseNumber')
BEGIN
    ALTER TABLE [REV].[ProcedureTransaction] ADD [CaseNumber] NVARCHAR(50) NULL;
    PRINT 'REV.ProcedureTransaction: added CaseNumber.';
END
GO

-- Rows that predate the column get a generated number the client can rename.
UPDATE [REV].[ProcedureTransaction]
SET [CaseNumber] = 'CASE-' + CAST([Id] AS NVARCHAR(20))
WHERE [CaseNumber] IS NULL OR LTRIM(RTRIM([CaseNumber])) = '';
GO

IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]')
             AND name = 'CaseNumber' AND is_nullable = 1)
BEGIN
    ALTER TABLE [REV].[ProcedureTransaction] ALTER COLUMN [CaseNumber] NVARCHAR(50) NOT NULL;
    PRINT 'REV.ProcedureTransaction: CaseNumber set to NOT NULL.';
END
GO

/* 2. Device line table ---------------------------------------------------------- */

IF NOT EXISTS (SELECT 1 FROM sys.objects
               WHERE object_id = OBJECT_ID(N'[REV].[ProcedureTransactionProduct]') AND type = N'U')
BEGIN
    CREATE TABLE [REV].[ProcedureTransactionProduct] (
        [Id]                     INT IDENTITY(1,1) NOT NULL,
        [ProcedureTransactionId] INT              NOT NULL,
        [ProductId]              INT              NOT NULL,
        [Quantity]               INT              NOT NULL,
        [UnitPrice]              DECIMAL(18,2)    NOT NULL,
        [LineTotal]              DECIMAL(18,2)    NOT NULL,
        [CreationTime]           DATETIME2        NOT NULL,
        [CreatorUserId]          BIGINT           NULL,
        [LastModificationTime]   DATETIME2        NULL,
        [LastModifierUserId]     BIGINT           NULL,
        [IsDeleted]              BIT              NOT NULL,
        [DeleterUserId]          BIGINT           NULL,
        [DeletionTime]           DATETIME2        NULL,
        CONSTRAINT [PK_ProcedureTransactionProduct] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProcedureTransactionProduct_ProcedureTransaction_ProcedureTransactionId]
            FOREIGN KEY ([ProcedureTransactionId])
            REFERENCES [REV].[ProcedureTransaction] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProcedureTransactionProduct_Product_ProductId]
            FOREIGN KEY ([ProductId])
            REFERENCES [REV].[Product] ([Id])
    );

    PRINT 'REV.ProcedureTransactionProduct: created.';
END
GO

/* 3. Carry each case's existing device onto a line ------------------------------ */

-- Guarded by dynamic SQL: the source columns may already be gone on a second run, and a
-- reference to a missing column fails at parse time even inside an IF that never runs.
IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]') AND name = 'ProductId')
BEGIN
    DECLARE @Sql NVARCHAR(MAX) = N'
        INSERT INTO [REV].[ProcedureTransactionProduct]
            ([ProcedureTransactionId], [ProductId], [Quantity], [UnitPrice], [LineTotal],
             [CreationTime], [CreatorUserId], [IsDeleted])
        SELECT
            pt.[Id],
            pt.[ProductId],
            CASE WHEN pt.[Quantity] > 0 THEN pt.[Quantity] ELSE 1 END,
            pt.[UnitPrice],
            pt.[UnitPrice] * CASE WHEN pt.[Quantity] > 0 THEN pt.[Quantity] ELSE 1 END,
            pt.[CreationTime],
            pt.[CreatorUserId],
            0
        FROM [REV].[ProcedureTransaction] pt
        WHERE pt.[ProductId] IS NOT NULL
          AND NOT EXISTS (SELECT 1 FROM [REV].[ProcedureTransactionProduct] l
                          WHERE l.[ProcedureTransactionId] = pt.[Id]);';

    EXEC sp_executesql @Sql;
    PRINT 'REV.ProcedureTransactionProduct: carried over the existing device on each case.';
END
GO

/* 4. Drop the old single-device columns ----------------------------------------- */

IF EXISTS (SELECT 1 FROM sys.foreign_keys
           WHERE name = N'FK_ProcedureTransaction_Product_ProductId')
BEGIN
    ALTER TABLE [REV].[ProcedureTransaction] DROP CONSTRAINT [FK_ProcedureTransaction_Product_ProductId];
END
GO

IF EXISTS (SELECT 1 FROM sys.indexes
           WHERE name = N'IX_ProcedureTransaction_ProductId'
             AND object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]'))
BEGIN
    DROP INDEX [IX_ProcedureTransaction_ProductId] ON [REV].[ProcedureTransaction];
END
GO

IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]') AND name = 'ProductId')
    ALTER TABLE [REV].[ProcedureTransaction] DROP COLUMN [ProductId];
GO

IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]') AND name = 'Quantity')
    ALTER TABLE [REV].[ProcedureTransaction] DROP COLUMN [Quantity];
GO

IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]') AND name = 'UnitPrice')
    ALTER TABLE [REV].[ProcedureTransaction] DROP COLUMN [UnitPrice];
GO

/* 5. Indexes ------------------------------------------------------------------- */

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = N'IX_ProcedureTransaction_CaseNumber'
                 AND object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]'))
BEGIN
    -- A duplicate case number would make two cases indistinguishable on every report.
    IF EXISTS (SELECT [CaseNumber] FROM [REV].[ProcedureTransaction]
               GROUP BY [CaseNumber] HAVING COUNT(*) > 1)
    BEGIN
        PRINT '=== Unique index NOT created: duplicate case numbers exist. ===';
        SELECT [CaseNumber], COUNT(*) AS Cases
        FROM [REV].[ProcedureTransaction]
        GROUP BY [CaseNumber] HAVING COUNT(*) > 1
        ORDER BY [CaseNumber];
    END
    ELSE
    BEGIN
        CREATE UNIQUE INDEX [IX_ProcedureTransaction_CaseNumber]
            ON [REV].[ProcedureTransaction] ([CaseNumber]);
        PRINT 'Created IX_ProcedureTransaction_CaseNumber.';
    END
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = N'IX_ProcedureTransactionProduct_ProcedureTransactionId'
                 AND object_id = OBJECT_ID(N'[REV].[ProcedureTransactionProduct]'))
    CREATE INDEX [IX_ProcedureTransactionProduct_ProcedureTransactionId]
        ON [REV].[ProcedureTransactionProduct] ([ProcedureTransactionId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = N'IX_ProcedureTransactionProduct_ProductId'
                 AND object_id = OBJECT_ID(N'[REV].[ProcedureTransactionProduct]'))
    CREATE INDEX [IX_ProcedureTransactionProduct_ProductId]
        ON [REV].[ProcedureTransactionProduct] ([ProductId]);
GO

/* Result ----------------------------------------------------------------------- */

SELECT
    pt.[Id]         AS CaseId,
    pt.[CaseNumber],
    pt.[ProcedureDate],
    pt.[ImplantType],
    pt.[TotalAmount],
    COUNT(l.[Id])   AS DeviceLines,
    ISNULL(SUM(l.[Quantity]), 0)  AS TotalUnits,
    ISNULL(SUM(l.[LineTotal]), 0) AS SumOfLines
FROM [REV].[ProcedureTransaction] pt
LEFT JOIN [REV].[ProcedureTransactionProduct] l
       ON l.[ProcedureTransactionId] = pt.[Id] AND l.[IsDeleted] = 0
GROUP BY pt.[Id], pt.[CaseNumber], pt.[ProcedureDate], pt.[ImplantType], pt.[TotalAmount]
ORDER BY pt.[ProcedureDate] DESC, pt.[Id];
GO

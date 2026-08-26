-- Add Status and Description to a case, and stop a deleted case number blocking reuse.
--
-- Two changes:
--
-- 1. Status (CaseStatus enum: 1 Open, 2 Scheduled, 3 Completed, 4 Billed, 5 Paid,
--    6 Closed) and Description. Existing cases default to Open. Recording the status
--    does not change any figure yet - reports still count cases regardless of status.
--
-- 2. IX_ProcedureTransaction_CaseNumber was created unfiltered, but
--    REV.ProcedureTransaction is soft-deleted. The application's duplicate check runs
--    through ABP's repository, which hides IsDeleted = 1 rows, so deleting case "100"
--    and entering it again passed that check and then failed at the database with
--    "Cannot insert duplicate key row ... The duplicate key value is (100)". Filtering
--    the index on IsDeleted = 0 makes the constraint agree with the check, and lets a
--    deleted case number be reused.
--
-- Re-runnable: every step is guarded.

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

SET NOCOUNT ON;

/* ---------------------------------------------------------------- Status */
IF NOT EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]') AND name = 'Status')
BEGIN
    ALTER TABLE [REV].[ProcedureTransaction]
        ADD [Status] INT NOT NULL CONSTRAINT [DF_ProcedureTransaction_Status] DEFAULT (1);
    PRINT 'REV.ProcedureTransaction: added Status (defaulted to Open).';
END
ELSE
    PRINT 'REV.ProcedureTransaction.Status already exists.';
GO

-- Any row that predates the column, or was written with 0, belongs on Open.
UPDATE [REV].[ProcedureTransaction] SET [Status] = 1 WHERE [Status] NOT BETWEEN 1 AND 6;
GO

/* ----------------------------------------------------------- Description */
IF NOT EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]') AND name = 'Description')
BEGIN
    ALTER TABLE [REV].[ProcedureTransaction] ADD [Description] NVARCHAR(1000) NULL;
    PRINT 'REV.ProcedureTransaction: added Description.';
END
ELSE
    PRINT 'REV.ProcedureTransaction.Description already exists.';
GO

/* ------------------------------------------- Filtered case number index */
IF EXISTS (SELECT 1 FROM sys.indexes
           WHERE name = N'IX_ProcedureTransaction_CaseNumber'
             AND object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]')
             AND has_filter = 0)
BEGIN
    DROP INDEX [IX_ProcedureTransaction_CaseNumber] ON [REV].[ProcedureTransaction];
    PRINT 'Dropped the unfiltered IX_ProcedureTransaction_CaseNumber.';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = N'IX_ProcedureTransaction_CaseNumber'
                 AND object_id = OBJECT_ID(N'[REV].[ProcedureTransaction]'))
BEGIN
    -- Duplicates among live rows would block the index; report them rather than fail.
    IF EXISTS (SELECT 1 FROM [REV].[ProcedureTransaction]
               WHERE IsDeleted = 0
               GROUP BY [CaseNumber] HAVING COUNT(*) > 1)
    BEGIN
        PRINT '=== Index NOT created: duplicate case numbers exist among live cases. ===';
        PRINT 'Resolve the rows below, then re-run this script.';

        SELECT [CaseNumber], COUNT(*) AS LiveCases
        FROM [REV].[ProcedureTransaction]
        WHERE IsDeleted = 0
        GROUP BY [CaseNumber]
        HAVING COUNT(*) > 1
        ORDER BY [CaseNumber];
    END
    ELSE
    BEGIN
        CREATE UNIQUE INDEX [IX_ProcedureTransaction_CaseNumber]
            ON [REV].[ProcedureTransaction] ([CaseNumber])
            WHERE [IsDeleted] = 0;
        PRINT 'Created IX_ProcedureTransaction_CaseNumber filtered on IsDeleted = 0.';
    END
END
ELSE
    PRINT 'IX_ProcedureTransaction_CaseNumber is already filtered - nothing to do.';
GO

SELECT [Status], COUNT(*) AS Cases
FROM [REV].[ProcedureTransaction]
WHERE IsDeleted = 0
GROUP BY [Status]
ORDER BY [Status];
GO

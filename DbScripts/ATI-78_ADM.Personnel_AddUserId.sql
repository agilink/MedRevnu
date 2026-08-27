-- Link a physician to the application login created for them.
--
-- Why this is needed: the physician grid can now create a login for a physician, and
-- something has to record which login belongs to which physician. That link is also what
-- the dashboard and the transaction list use to work out which hospital's data a
-- physician user is allowed to see, so without it a physician login would see everything.
--
-- AbpUsers.Id is a BIGINT, so this column is a BIGINT.
--
-- Deliberately not a foreign key: users are soft-deleted and managed by the framework,
-- and a personnel record should outlive its login rather than be cascaded away with it.
-- The filtered unique index is what actually matters - it stops the same login being
-- attached to two physicians.
--
-- Re-runnable: the column and the index are each created only if absent.

SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[ADM].[Personnel]') AND name = 'UserId'
)
BEGIN
    ALTER TABLE [ADM].[Personnel] ADD [UserId] BIGINT NULL;
    PRINT 'Added ADM.Personnel.UserId.';
END
ELSE
BEGIN
    PRINT 'ADM.Personnel.UserId already present.';
END
GO

SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;

-- Filtered so the many physicians with no login do not all collide on NULL, and so
-- soft-deleted personnel do not hold a login hostage.
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'[ADM].[Personnel]') AND name = 'IX_Personnel_UserId'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Personnel_UserId]
        ON [ADM].[Personnel] ([UserId])
        WHERE [UserId] IS NOT NULL AND [IsDeleted] = 0;

    PRINT 'Created IX_Personnel_UserId.';
END
ELSE
BEGIN
    PRINT 'IX_Personnel_UserId already present.';
END
GO

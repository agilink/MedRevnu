-- ============================================================================
-- SQL Scripts to Fix NULL CreationTime Values in ProductCategory Table
-- ============================================================================
-- These scripts will update any NULL CreationTime values in the ProductCategory table
-- Run these if you still encounter DateTime issues with ProductCategory records

-- 1. Check for NULL CreationTime values
SELECT
    Id,
    Name,
    CreationTime,
    LastModificationTime
FROM [REV].[ProductCategory]
WHERE CreationTime IS NULL;

-- 2. Update NULL CreationTime values to current UTC date/time
-- (You can adjust the date to a more appropriate value if needed)
UPDATE [REV].[ProductCategory]
SET CreationTime = GETUTCDATE()
WHERE CreationTime IS NULL;

-- 3. Verify the update
SELECT
    Id,
    Name,
    CreationTime,
    LastModificationTime
FROM [REV].[ProductCategory]
ORDER BY Id;

-- ============================================================================
-- Note: The code changes already fix the ProductQuotas page by using a
-- projection query that only selects Id and Name fields, avoiding the
-- CreationTime field entirely. These SQL scripts are optional cleanup for
-- data integrity.
-- ============================================================================

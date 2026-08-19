-- Prevent duplicate hospital/product price rows for the same effective date.
--
-- Why: GetProductPriceByHospital picks the newest active row for a hospital +
-- product. With no uniqueness constraint, two rows carrying the same effective
-- date make that choice arbitrary, so the same case can price differently
-- depending on row order. EffectiveDate is nullable and SQL Server treats NULLs
-- as equal in a unique index, which gives us exactly one "no effective date"
-- baseline row per hospital + product.
--
-- Re-runnable: creates the index only when absent, and refuses to create it
-- while duplicates still exist rather than failing the deployment.

SET NOCOUNT ON;

IF EXISTS (SELECT 1 FROM sys.indexes
           WHERE name = N'UX_HospitalProductPrice_Hospital_Product_EffectiveDate'
             AND object_id = OBJECT_ID(N'[REV].[HospitalProductPrice]'))
BEGIN
    PRINT 'UX_HospitalProductPrice_Hospital_Product_EffectiveDate already exists - nothing to do.';
END
ELSE
BEGIN
    DECLARE @DuplicateCount INT;

    SELECT @DuplicateCount = COUNT(*)
    FROM (
        SELECT HospitalId, ProductId, EffectiveDate
        FROM [REV].[HospitalProductPrice]
        GROUP BY HospitalId, ProductId, EffectiveDate
        HAVING COUNT(*) > 1
    ) d;

    IF @DuplicateCount > 0
    BEGIN
        PRINT '=== Unique index NOT created: duplicate rows must be resolved first. ===';
        PRINT 'The rows below share a HospitalId + ProductId + EffectiveDate.';
        PRINT 'Keep the correct price for each group, delete the rest, then re-run this script.';

        SELECT
            hpp.HospitalId,
            hpp.ProductId,
            hpp.EffectiveDate,
            COUNT(*)          AS DuplicateRows,
            MIN(hpp.UnitPrice) AS MinUnitPrice,
            MAX(hpp.UnitPrice) AS MaxUnitPrice
        FROM [REV].[HospitalProductPrice] hpp
        GROUP BY hpp.HospitalId, hpp.ProductId, hpp.EffectiveDate
        HAVING COUNT(*) > 1
        ORDER BY hpp.HospitalId, hpp.ProductId, hpp.EffectiveDate;
    END
    ELSE
    BEGIN
        CREATE UNIQUE INDEX [UX_HospitalProductPrice_Hospital_Product_EffectiveDate]
            ON [REV].[HospitalProductPrice] ([HospitalId], [ProductId], [EffectiveDate]);

        PRINT 'Created UX_HospitalProductPrice_Hospital_Product_EffectiveDate.';
    END
END
GO

-- Supporting index for the price lookup (hospital + product, active rows, newest first).
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = N'IX_HospitalProductPrice_HospitalId_ProductId_IsActive'
                 AND object_id = OBJECT_ID(N'[REV].[HospitalProductPrice]'))
BEGIN
    CREATE INDEX [IX_HospitalProductPrice_HospitalId_ProductId_IsActive]
        ON [REV].[HospitalProductPrice] ([HospitalId], [ProductId], [IsActive])
        INCLUDE ([EffectiveDate], [UnitPrice]);

    PRINT 'Created IX_HospitalProductPrice_HospitalId_ProductId_IsActive.';
END
ELSE
BEGIN
    PRINT 'IX_HospitalProductPrice_HospitalId_ProductId_IsActive already exists - nothing to do.';
END
GO

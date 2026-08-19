-- Report transactions whose ImplantType contradicts the product they record.
--
-- Why: De Novo and Gen Change are separate product subcategories ("Single Chamber"
-- vs "Single Chamber Gen Change"), so a product already determines which one a
-- transaction is. Nothing tied the entry form's radio button to the product chosen,
-- so a battery replacement could be recorded against a de novo device - and that
-- mismatch lands straight in the De Novo / Gen Change columns the business reports
-- on. The application now rejects a mismatch on save; this finds any that were
-- recorded before that check existed.
--
-- Read-only and re-runnable. Correct anything listed here on the Revenue
-- Transactions screen, by fixing whichever of the two fields is wrong.

SET NOCOUNT ON;

DECLARE @Mismatched INT, @NoSubcategory INT;

SELECT @Mismatched = COUNT(*)
FROM [REV].[ProcedureTransaction] pt
INNER JOIN [REV].[Product] p             ON p.Id  = pt.ProductId
INNER JOIN [REV].[ProductSubcategory] sc ON sc.Id = p.SubproductCategoryId
WHERE sc.ImplantType <> pt.ImplantType;

SELECT @NoSubcategory = COUNT(*)
FROM [REV].[ProcedureTransaction] pt
INNER JOIN [REV].[Product] p ON p.Id = pt.ProductId
WHERE p.SubproductCategoryId IS NULL;

PRINT 'Transactions with a contradicting implant type: ' + CAST(@Mismatched AS VARCHAR(20));
PRINT 'Transactions on a product with no subcategory (cannot be checked): ' + CAST(@NoSubcategory AS VARCHAR(20));

IF @Mismatched > 0
BEGIN
    SELECT
        pt.Id                AS TransactionId,
        pt.ProcedureDate,
        f.FacilityName       AS Hospital,
        p.ProductCode,
        p.Name               AS ProductName,
        sc.SubcategoryName,
        CASE sc.ImplantType WHEN 2 THEN 'GenChange' ELSE 'DeNovo' END AS ProductImplies,
        CASE pt.ImplantType WHEN 2 THEN 'GenChange' ELSE 'DeNovo' END AS RecordedAs,
        pt.Quantity,
        pt.TotalAmount
    FROM [REV].[ProcedureTransaction] pt
    INNER JOIN [REV].[Product] p             ON p.Id  = pt.ProductId
    INNER JOIN [REV].[ProductSubcategory] sc ON sc.Id = p.SubproductCategoryId
    LEFT  JOIN [ADM].[Facility] f            ON f.Id  = pt.HospitalId
    WHERE sc.ImplantType <> pt.ImplantType
    ORDER BY pt.ProcedureDate DESC, pt.Id;
END

IF @NoSubcategory > 0
BEGIN
    PRINT 'Products below have no subcategory, so their implant type cannot be verified.';
    PRINT 'Assign each one a subcategory on the Products screen.';

    SELECT DISTINCT
        p.Id AS ProductId,
        p.ProductCode,
        p.Name AS ProductName
    FROM [REV].[ProcedureTransaction] pt
    INNER JOIN [REV].[Product] p ON p.Id = pt.ProductId
    WHERE p.SubproductCategoryId IS NULL
    ORDER BY p.Name;
END

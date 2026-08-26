-- Report device lines whose product contradicts the implant type of their case.
--
-- Why: De Novo and Gen Change are separate product subcategories ("Single Chamber" vs
-- "Single Chamber Gen Change"), so a product already implies which one it is. A case
-- carries one implant type and every device on it must match, otherwise the mismatch
-- lands straight in the De Novo / Gen Change columns the business reports on. The
-- application rejects a mismatch on save; this finds any recorded before that check
-- existed.
--
-- Devices live on REV.ProcedureTransactionProduct, one row per device, since a
-- procedure can involve two or three of them.
--
-- Read-only and re-runnable. Fix anything listed here on the Revenue Transactions
-- screen, by correcting whichever of the two fields is wrong.

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

SET NOCOUNT ON;

DECLARE @Mismatched INT, @NoSubcategory INT;

SELECT @Mismatched = COUNT(*)
FROM [REV].[ProcedureTransactionProduct] l
INNER JOIN [REV].[ProcedureTransaction] pt ON pt.Id = l.ProcedureTransactionId
INNER JOIN [REV].[Product] p               ON p.Id  = l.ProductId
INNER JOIN [REV].[ProductSubcategory] sc   ON sc.Id = p.SubproductCategoryId
WHERE pt.IsDeleted = 0
  AND l.IsDeleted = 0
  AND sc.ImplantType <> pt.ImplantType;

SELECT @NoSubcategory = COUNT(*)
FROM [REV].[ProcedureTransactionProduct] l
INNER JOIN [REV].[ProcedureTransaction] pt ON pt.Id = l.ProcedureTransactionId
INNER JOIN [REV].[Product] p               ON p.Id  = l.ProductId
WHERE pt.IsDeleted = 0
  AND l.IsDeleted = 0
  AND p.SubproductCategoryId IS NULL;

PRINT 'Device lines contradicting their case implant type: ' + CAST(@Mismatched AS VARCHAR(20));
PRINT 'Device lines on a product with no subcategory (cannot be checked): ' + CAST(@NoSubcategory AS VARCHAR(20));

IF @Mismatched > 0
BEGIN
    SELECT
        pt.Id            AS CaseId,
        pt.CaseNumber,
        pt.ProcedureDate,
        f.FacilityName   AS Hospital,
        p.ProductCode,
        p.Name           AS ProductName,
        sc.SubcategoryName,
        CASE sc.ImplantType WHEN 2 THEN 'GenChange' ELSE 'DeNovo' END AS DeviceImplies,
        CASE pt.ImplantType WHEN 2 THEN 'GenChange' ELSE 'DeNovo' END AS CaseRecordedAs,
        l.Quantity,
        l.LineTotal
    FROM [REV].[ProcedureTransactionProduct] l
    INNER JOIN [REV].[ProcedureTransaction] pt ON pt.Id = l.ProcedureTransactionId
    INNER JOIN [REV].[Product] p               ON p.Id  = l.ProductId
    INNER JOIN [REV].[ProductSubcategory] sc   ON sc.Id = p.SubproductCategoryId
    LEFT  JOIN [ADM].[Facility] f              ON f.Id  = pt.HospitalId
    WHERE pt.IsDeleted = 0
      AND l.IsDeleted = 0
      AND sc.ImplantType <> pt.ImplantType
    ORDER BY pt.ProcedureDate DESC, pt.CaseNumber, p.Name;
END

IF @NoSubcategory > 0
BEGIN
    PRINT 'The products below have no subcategory, so their implant type cannot be verified.';
    PRINT 'Assign each one a subcategory on the Products screen.';

    SELECT DISTINCT
        p.Id AS ProductId,
        p.ProductCode,
        p.Name AS ProductName
    FROM [REV].[ProcedureTransactionProduct] l
    INNER JOIN [REV].[ProcedureTransaction] pt ON pt.Id = l.ProcedureTransactionId
    INNER JOIN [REV].[Product] p               ON p.Id  = l.ProductId
    WHERE pt.IsDeleted = 0
      AND l.IsDeleted = 0
      AND p.SubproductCategoryId IS NULL
    ORDER BY p.Name;
END
GO

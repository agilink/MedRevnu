-- Report what REV.Case and REV.CaseProduct still hold after the code was retired.
--
-- Why: ProcedureTransaction is now the single revenue model, so the Case / CaseProduct
-- entities, services, pages and menu entry were removed. The tables were deliberately
-- left in place - retiring the code does not require discarding the rows, and dropping
-- a table cannot be undone.
--
-- Use this to decide what happens to the contents. Anything worth keeping should be
-- re-entered on Revenue > Revenue Transactions, which records the same facts
-- (hospital, physician, product, date, quantity, unit price, amount) plus the implant
-- type the reports need. Once the client confirms nothing here is needed, the two
-- tables can be dropped in a separate script.
--
-- Read-only and re-runnable. It drops nothing.

SET NOCOUNT ON;

IF OBJECT_ID(N'[REV].[Case]', N'U') IS NULL
BEGIN
    PRINT 'REV.Case does not exist - nothing retained.';
    RETURN;
END

DECLARE @Cases INT, @CaseProducts INT, @CaseTotal DECIMAL(18,2), @ProductTotal DECIMAL(18,2);

SELECT @Cases = COUNT(*), @CaseTotal = ISNULL(SUM(TotalAmount), 0) FROM [REV].[Case];
SELECT @CaseProducts = COUNT(*), @ProductTotal = ISNULL(SUM(TotalPrice), 0) FROM [REV].[CaseProduct];

PRINT 'REV.Case rows: ' + CAST(@Cases AS VARCHAR(20))
    + ' (TotalAmount sum ' + CAST(@CaseTotal AS VARCHAR(30)) + ')';
PRINT 'REV.CaseProduct rows: ' + CAST(@CaseProducts AS VARCHAR(20))
    + ' (TotalPrice sum ' + CAST(@ProductTotal AS VARCHAR(30)) + ')';

IF @Cases = 0
BEGIN
    PRINT 'Both tables are empty - they can be dropped whenever convenient.';
    RETURN;
END

-- Case headers, with the sum of their products alongside the stored total. The two
-- disagreeing is expected: Case.TotalAmount was entered by hand and never recalculated
-- from its products, which is one of the reasons this model was retired.
SELECT
    c.Id                AS CaseId,
    c.CaseNumber,
    c.ClientName,
    c.CaseDate,
    c.ProcedureDate,
    c.Status,
    c.SurgeonName,
    f.FacilityName      AS Hospital,
    pt.Name             AS ProcedureTypeName,
    c.TotalAmount       AS StoredTotalAmount,
    ISNULL(cp.ProductTotal, 0) AS SumOfCaseProducts,
    c.TotalAmount - ISNULL(cp.ProductTotal, 0) AS Discrepancy,
    c.Notes
FROM [REV].[Case] c
LEFT JOIN [ADM].[Facility] f       ON f.Id  = c.FacilityId
LEFT JOIN [REV].[ProcedureType] pt ON pt.Id = c.ProcedureTypeId
LEFT JOIN (
    SELECT CaseId, SUM(TotalPrice) AS ProductTotal
    FROM [REV].[CaseProduct]
    GROUP BY CaseId
) cp ON cp.CaseId = c.Id
ORDER BY c.CaseDate DESC, c.Id;

-- Line items, so they can be re-entered as transactions if needed.
SELECT
    cp.CaseId,
    c.CaseNumber,
    p.ProductCode,
    p.Name       AS ProductName,
    sc.SubcategoryName,
    CASE sc.ImplantType WHEN 2 THEN 'GenChange' WHEN 1 THEN 'DeNovo' ELSE '(unknown)' END AS ImplantTypeFromProduct,
    cp.Quantity,
    cp.UnitPrice,
    cp.Discount,
    cp.TotalPrice
FROM [REV].[CaseProduct] cp
INNER JOIN [REV].[Case] c                ON c.Id  = cp.CaseId
LEFT  JOIN [REV].[Product] p             ON p.Id  = cp.ProductId
LEFT  JOIN [REV].[ProductSubcategory] sc ON sc.Id = p.SubproductCategoryId
ORDER BY cp.CaseId, cp.Id;

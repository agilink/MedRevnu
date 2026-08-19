-- Report any REV.ProcedureQuota rows that need re-entering as ProductQuota rows.
--
-- Why: the application had two quota models. REV.ProcedureQuota is keyed on
-- ProcedureType x Facility x date range and was what the dashboard's
-- quota-vs-actual read, while REV.ProductQuota is keyed on Hospital x
-- ProductCategory x Month and is the only quota screen in the menu. Targets
-- entered by a user therefore never reached the dashboard.
--
-- ProductQuota is now the single quota model. This script does NOT migrate the
-- data automatically: ProcedureQuota is keyed on ProcedureType (whose
-- CategoryGroup enum is Pacemaker / Defibrillator / BatteryChange... ) whereas
-- ProductQuota is keyed on ProductCategory (CRM / ICD / Leadless / Other).
-- There is no reliable one-to-one mapping between them, and guessing would put
-- wrong targets in front of the client. Anything listed below must be re-entered
-- on the Product Quotas screen.
--
-- Read-only and re-runnable. It drops nothing.

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.objects
               WHERE object_id = OBJECT_ID(N'[REV].[ProcedureQuota]') AND type = N'U')
BEGIN
    PRINT 'REV.ProcedureQuota does not exist - nothing to retire.';
    RETURN;
END

DECLARE @RowCount INT;
SELECT @RowCount = COUNT(*) FROM [REV].[ProcedureQuota];

IF @RowCount = 0
BEGIN
    PRINT 'REV.ProcedureQuota is empty. The table can be dropped once the code that';
    PRINT 'references it is removed - no data needs re-entering.';
END
ELSE
BEGIN
    PRINT '=== ' + CAST(@RowCount AS VARCHAR(20)) + ' ProcedureQuota row(s) need re-entering as ProductQuota. ===';
    PRINT 'Re-enter each row below on Revenue > Product Quotas, choosing the product';
    PRINT 'category that corresponds to the procedure type, then drop this table.';

    SELECT
        pq.Id                AS ProcedureQuotaId,
        pt.Name              AS ProcedureTypeName,
        pt.Code              AS ProcedureTypeCode,
        pt.CategoryGroup     AS CategoryGroup,
        pq.FacilityId,
        f.FacilityName,
        pq.QuotaPeriod,
        pq.QuotaValue,
        pq.StartDate,
        pq.EndDate,
        pq.Notes
    FROM [REV].[ProcedureQuota] pq
    LEFT JOIN [REV].[ProcedureType] pt ON pt.Id = pq.ProcedureTypeId
    LEFT JOIN [ADM].[Facility]     f  ON f.Id  = pq.FacilityId
    ORDER BY f.FacilityName, pt.Name, pq.StartDate;
END
GO

-- Seed sample Facilities for dropdown
-- Re-runnable: inserts only if FacilityName does not already exist

IF NOT EXISTS (SELECT 1 FROM [ADM].[Facility] WHERE FacilityName = 'Memorial Regional Hospital' AND IsDeleted = 0)
    INSERT INTO [ADM].[Facility] (FacilityName, AddressId, FacilityStatusId, CompanyId, CreationTime, IsDeleted)
    VALUES ('Memorial Regional Hospital', NULL, NULL, NULL, GETUTCDATE(), 0);

IF NOT EXISTS (SELECT 1 FROM [ADM].[Facility] WHERE FacilityName = 'St. Mary Medical Center' AND IsDeleted = 0)
    INSERT INTO [ADM].[Facility] (FacilityName, AddressId, FacilityStatusId, CompanyId, CreationTime, IsDeleted)
    VALUES ('St. Mary Medical Center', NULL, NULL, NULL, GETUTCDATE(), 0);

IF NOT EXISTS (SELECT 1 FROM [ADM].[Facility] WHERE FacilityName = 'General Health System' AND IsDeleted = 0)
    INSERT INTO [ADM].[Facility] (FacilityName, AddressId, FacilityStatusId, CompanyId, CreationTime, IsDeleted)
    VALUES ('General Health System', NULL, NULL, NULL, GETUTCDATE(), 0);

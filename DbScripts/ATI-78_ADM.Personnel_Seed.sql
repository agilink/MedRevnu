-- Seed sample Personnel (surgeons) for dropdown
-- Re-runnable: inserts only if EMPLOYEE_ID does not already exist

IF NOT EXISTS (SELECT 1 FROM [ADM].[Personnel] WHERE EMPLOYEE_ID = 'EMP-001' AND IsDeleted = 0)
    INSERT INTO [ADM].[Personnel]
        (FIRST_NAME, FIRST_NAME_PROPER, MIDDLE_NAME, LAST_NAME, SSN, EMPLOYEE_ID,
         COMPANY_NAME, ADDRESS1, ADDRESS2, CITY, ZIP_CODE,
         NUMBER_HOME, NUMBER_MOBILE, EMAIL_WORK, EMAIL_OTHER,
         NOTES, MODIFIED_BY,
         CreationTime, IsDeleted)
    VALUES
        ('James', 'James', '', 'Smith', '000-00-0001', 'EMP-001',
         'ATI Medical', '100 Main St', '', 'Miami', '33101',
         '', '', 'j.smith@hospital.com', '',
         '', 'system',
         GETUTCDATE(), 0);

IF NOT EXISTS (SELECT 1 FROM [ADM].[Personnel] WHERE EMPLOYEE_ID = 'EMP-002' AND IsDeleted = 0)
    INSERT INTO [ADM].[Personnel]
        (FIRST_NAME, FIRST_NAME_PROPER, MIDDLE_NAME, LAST_NAME, SSN, EMPLOYEE_ID,
         COMPANY_NAME, ADDRESS1, ADDRESS2, CITY, ZIP_CODE,
         NUMBER_HOME, NUMBER_MOBILE, EMAIL_WORK, EMAIL_OTHER,
         NOTES, MODIFIED_BY,
         CreationTime, IsDeleted)
    VALUES
        ('Sarah', 'Sarah', '', 'Johnson', '000-00-0002', 'EMP-002',
         'ATI Medical', '200 Oak Ave', '', 'Miami', '33102',
         '', '', 's.johnson@hospital.com', '',
         '', 'system',
         GETUTCDATE(), 0);

IF NOT EXISTS (SELECT 1 FROM [ADM].[Personnel] WHERE EMPLOYEE_ID = 'EMP-003' AND IsDeleted = 0)
    INSERT INTO [ADM].[Personnel]
        (FIRST_NAME, FIRST_NAME_PROPER, MIDDLE_NAME, LAST_NAME, SSN, EMPLOYEE_ID,
         COMPANY_NAME, ADDRESS1, ADDRESS2, CITY, ZIP_CODE,
         NUMBER_HOME, NUMBER_MOBILE, EMAIL_WORK, EMAIL_OTHER,
         NOTES, MODIFIED_BY,
         CreationTime, IsDeleted)
    VALUES
        ('Michael', 'Michael', '', 'Williams', '000-00-0003', 'EMP-003',
         'ATI Medical', '300 Pine Rd', '', 'Miami', '33103',
         '', '', 'm.williams@hospital.com', '',
         '', 'system',
         GETUTCDATE(), 0);

-- Seed active Procedure Types for Revenue module
-- Re-runnable: inserts only if Code does not already exist

-- CategoryGroup values: Pacemaker=1, Defibrillator=2, BatteryChangePacemaker=3, BatteryChangeICD=4, Leadless=5

IF NOT EXISTS (SELECT 1 FROM [REV].[ProcedureType] WHERE Code = 'PM-STD')
    INSERT INTO [REV].[ProcedureType] (Name, Code, CategoryGroup, Description, IsActive, DisplayOrder, CreationTime, LastModificationTime, CreatorUserId, LastModifierUserId)
    VALUES ('Standard Pacemaker', 'PM-STD', 1, 'Standard single or dual-chamber pacemaker implant', 1, 10, GETUTCDATE(), NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM [REV].[ProcedureType] WHERE Code = 'PM-DUAL')
    INSERT INTO [REV].[ProcedureType] (Name, Code, CategoryGroup, Description, IsActive, DisplayOrder, CreationTime, LastModificationTime, CreatorUserId, LastModifierUserId)
    VALUES ('Dual Chamber Pacemaker', 'PM-DUAL', 1, 'Dual-chamber pacemaker implant', 1, 20, GETUTCDATE(), NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM [REV].[ProcedureType] WHERE Code = 'ICD-STD')
    INSERT INTO [REV].[ProcedureType] (Name, Code, CategoryGroup, Description, IsActive, DisplayOrder, CreationTime, LastModificationTime, CreatorUserId, LastModifierUserId)
    VALUES ('Standard ICD', 'ICD-STD', 2, 'Standard implantable cardioverter-defibrillator', 1, 30, GETUTCDATE(), NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM [REV].[ProcedureType] WHERE Code = 'ICD-CRT')
    INSERT INTO [REV].[ProcedureType] (Name, Code, CategoryGroup, Description, IsActive, DisplayOrder, CreationTime, LastModificationTime, CreatorUserId, LastModifierUserId)
    VALUES ('CRT-D Defibrillator', 'ICD-CRT', 2, 'Cardiac resynchronization therapy defibrillator', 1, 40, GETUTCDATE(), NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM [REV].[ProcedureType] WHERE Code = 'BATT-PM')
    INSERT INTO [REV].[ProcedureType] (Name, Code, CategoryGroup, Description, IsActive, DisplayOrder, CreationTime, LastModificationTime, CreatorUserId, LastModifierUserId)
    VALUES ('Battery Change - Pacemaker', 'BATT-PM', 3, 'Pacemaker battery replacement', 1, 50, GETUTCDATE(), NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM [REV].[ProcedureType] WHERE Code = 'BATT-ICD')
    INSERT INTO [REV].[ProcedureType] (Name, Code, CategoryGroup, Description, IsActive, DisplayOrder, CreationTime, LastModificationTime, CreatorUserId, LastModifierUserId)
    VALUES ('Battery Change - ICD', 'BATT-ICD', 4, 'ICD battery replacement', 1, 60, GETUTCDATE(), NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM [REV].[ProcedureType] WHERE Code = 'LEAD-PM')
    INSERT INTO [REV].[ProcedureType] (Name, Code, CategoryGroup, Description, IsActive, DisplayOrder, CreationTime, LastModificationTime, CreatorUserId, LastModifierUserId)
    VALUES ('Leadless Pacemaker', 'LEAD-PM', 5, 'Leadless pacemaker implant', 1, 70, GETUTCDATE(), NULL, NULL, NULL);

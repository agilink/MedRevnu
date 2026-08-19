-- Seed Product Categories and Products for Revenue module
-- Re-runnable: uses IF NOT EXISTS guards

-- -------------------------------------------------------
-- Product Categories
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [REV].[ProductCategory] WHERE Name = 'Pacemaker Devices')
    INSERT INTO [REV].[ProductCategory] (Name, ShortDescription, Description, CreationTime, LastModificationTime, CreatorUserId, LastModifierUserId)
    VALUES ('Pacemaker Devices', 'Pacemakers', 'Pacemaker devices and accessories', GETUTCDATE(), NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM [REV].[ProductCategory] WHERE Name = 'Defibrillator Devices')
    INSERT INTO [REV].[ProductCategory] (Name, ShortDescription, Description, CreationTime, LastModificationTime, CreatorUserId, LastModifierUserId)
    VALUES ('Defibrillator Devices', 'ICD/CRT-D', 'Implantable defibrillator devices and accessories', GETUTCDATE(), NULL, NULL, NULL);

-- -------------------------------------------------------
-- Products  (CategoryId resolved by sub-select)
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [REV].[Product] WHERE ProductCode = 'PM-001')
    INSERT INTO [REV].[Product]
        (ProductCode, Name, Manufacturer, ModelNo, Description, ProductCategoryId,
         BasePrice, Cost, Price, IsActive, IsSystem, CreationTime, LastModificationTime, CreatorUserId, LastModifierUserId)
    VALUES
        ('PM-001', 'Medtronic Micra AV', 'Medtronic', 'ATPM01', 'Leadless pacemaker AV synchrony',
         (SELECT TOP 1 Id FROM [REV].[ProductCategory] WHERE Name = 'Pacemaker Devices'),
         8500.00, 7000.00, 8500.00, 1, 0, GETUTCDATE(), NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM [REV].[Product] WHERE ProductCode = 'PM-002')
    INSERT INTO [REV].[Product]
        (ProductCode, Name, Manufacturer, ModelNo, Description, ProductCategoryId,
         BasePrice, Cost, Price, IsActive, IsSystem, CreationTime, LastModificationTime, CreatorUserId, LastModifierUserId)
    VALUES
        ('PM-002', 'Abbott Gallant HF', 'Abbott', 'GAHF01', 'Dual-chamber pacemaker with HF monitoring',
         (SELECT TOP 1 Id FROM [REV].[ProductCategory] WHERE Name = 'Pacemaker Devices'),
         7200.00, 5800.00, 7200.00, 1, 0, GETUTCDATE(), NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM [REV].[Product] WHERE ProductCode = 'ICD-001')
    INSERT INTO [REV].[Product]
        (ProductCode, Name, Manufacturer, ModelNo, Description, ProductCategoryId,
         BasePrice, Cost, Price, IsActive, IsSystem, CreationTime, LastModificationTime, CreatorUserId, LastModifierUserId)
    VALUES
        ('ICD-001', 'Boston Scientific EMBLEM MRI', 'Boston Scientific', 'EBL01', 'Subcutaneous ICD with MRI compatibility',
         (SELECT TOP 1 Id FROM [REV].[ProductCategory] WHERE Name = 'Defibrillator Devices'),
         12000.00, 9500.00, 12000.00, 1, 0, GETUTCDATE(), NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM [REV].[Product] WHERE ProductCode = 'ICD-002')
    INSERT INTO [REV].[Product]
        (ProductCode, Name, Manufacturer, ModelNo, Description, ProductCategoryId,
         BasePrice, Cost, Price, IsActive, IsSystem, CreationTime, LastModificationTime, CreatorUserId, LastModifierUserId)
    VALUES
        ('ICD-002', 'Medtronic Evoque CRT-D', 'Medtronic', 'EVQD01', 'Cardiac resynchronization therapy defibrillator',
         (SELECT TOP 1 Id FROM [REV].[ProductCategory] WHERE Name = 'Defibrillator Devices'),
         18500.00, 14000.00, 18500.00, 1, 0, GETUTCDATE(), NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM [REV].[Product] WHERE ProductCode = 'LEAD-001')
    INSERT INTO [REV].[Product]
        (ProductCode, Name, Manufacturer, ModelNo, Description, ProductCategoryId,
         BasePrice, Cost, Price, IsActive, IsSystem, CreationTime, LastModificationTime, CreatorUserId, LastModifierUserId)
    VALUES
        ('LEAD-001', 'Medtronic SelectSecure Lead', 'Medtronic', 'SSL01', 'Lumenless pacing lead',
         (SELECT TOP 1 Id FROM [REV].[ProductCategory] WHERE Name = 'Pacemaker Devices'),
         1200.00, 950.00, 1200.00, 1, 0, GETUTCDATE(), NULL, NULL, NULL);

-- Seed data for 21 Procedure Types
-- CategoryGroup enum: Pacemaker=1, Defibrillator=2, BatteryChangePacemaker=3, BatteryChangeICD=4, Leadless=5

SET IDENTITY_INSERT [REV].[ProcedureType] ON;

-- Pacemaker procedures (CategoryGroup = 1)
INSERT INTO [REV].[ProcedureType] ([Id], [Name], [Code], [CategoryGroup], [Description], [IsActive], [DisplayOrder], [CreationTime])
VALUES
(1, 'Pacemaker - Single Chamber', 'PM-SC', 1, 'Single chamber pacemaker implantation', 1, 1, GETDATE()),
(2, 'Pacemaker - Dual Chamber', 'PM-DC', 1, 'Dual chamber pacemaker implantation', 1, 2, GETDATE()),
(3, 'Pacemaker - Dual Chamber Left Bundle', 'PM-DCLB', 1, 'Dual chamber pacemaker with left bundle branch pacing', 1, 3, GETDATE()),
(4, 'Pacemaker - CRT-P', 'PM-CRTP', 1, 'Cardiac resynchronization therapy pacemaker', 1, 4, GETDATE()),
(5, 'Pacemaker - CRT-P Left Bundle', 'PM-CRTPLB', 1, 'CRT pacemaker with left bundle branch pacing', 1, 5, GETDATE());

-- Defibrillator procedures (CategoryGroup = 2)
INSERT INTO [REV].[ProcedureType] ([Id], [Name], [Code], [CategoryGroup], [Description], [IsActive], [DisplayOrder], [CreationTime])
VALUES
(6, 'ICD - Single Chamber', 'ICD-SC', 2, 'Single chamber implantable cardioverter defibrillator', 1, 6, GETDATE()),
(7, 'ICD - Dual Chamber', 'ICD-DC', 2, 'Dual chamber implantable cardioverter defibrillator', 1, 7, GETDATE()),
(8, 'ICD - CRT-D', 'ICD-CRTD', 2, 'Cardiac resynchronization therapy defibrillator', 1, 8, GETDATE()),
(9, 'ICD - CRT-D Left Bundle', 'ICD-CRTDLB', 2, 'CRT defibrillator with left bundle branch pacing', 1, 9, GETDATE());

-- Battery Change - Pacemaker (CategoryGroup = 3)
INSERT INTO [REV].[ProcedureType] ([Id], [Name], [Code], [CategoryGroup], [Description], [IsActive], [DisplayOrder], [CreationTime])
VALUES
(10, 'Battery Change - Pacemaker Single Chamber', 'BC-PM-SC', 3, 'Generator change for single chamber pacemaker', 1, 10, GETDATE()),
(11, 'Battery Change - Pacemaker Dual Chamber', 'BC-PM-DC', 3, 'Generator change for dual chamber pacemaker', 1, 11, GETDATE()),
(12, 'Battery Change - CRT-P', 'BC-CRTP', 3, 'Generator change for CRT pacemaker', 1, 12, GETDATE());

-- Battery Change - ICD (CategoryGroup = 4)
INSERT INTO [REV].[ProcedureType] ([Id], [Name], [Code], [CategoryGroup], [Description], [IsActive], [DisplayOrder], [CreationTime])
VALUES
(13, 'Battery Change - ICD Single Chamber', 'BC-ICD-SC', 4, 'Generator change for single chamber ICD', 1, 13, GETDATE()),
(14, 'Battery Change - ICD Dual Chamber', 'BC-ICD-DC', 4, 'Generator change for dual chamber ICD', 1, 14, GETDATE()),
(15, 'Battery Change - CRT-D', 'BC-CRTD', 4, 'Generator change for CRT defibrillator', 1, 15, GETDATE());

-- Leadless procedures (CategoryGroup = 5)
INSERT INTO [REV].[ProcedureType] ([Id], [Name], [Code], [CategoryGroup], [Description], [IsActive], [DisplayOrder], [CreationTime])
VALUES
(16, 'Leadless - AR', 'LL-AR', 5, 'Leadless pacemaker - atrial', 1, 16, GETDATE()),
(17, 'Leadless - VR', 'LL-VR', 5, 'Leadless pacemaker - ventricular', 1, 17, GETDATE()),
(18, 'Leadless - DR', 'LL-DR', 5, 'Leadless pacemaker - dual chamber', 1, 18, GETDATE()),
(19, 'Leadless - AR Battery Change', 'LL-AR-BC', 5, 'Battery change for leadless pacemaker - atrial', 1, 19, GETDATE()),
(20, 'Leadless - VR Battery Change', 'LL-VR-BC', 5, 'Battery change for leadless pacemaker - ventricular', 1, 20, GETDATE()),
(21, 'Leadless - DR Battery Change', 'LL-DR-BC', 5, 'Battery change for leadless pacemaker - dual chamber', 1, 21, GETDATE());

SET IDENTITY_INSERT [REV].[ProcedureType] OFF;

SELECT COUNT(*) as 'Total Procedure Types Inserted' FROM [REV].[ProcedureType];

-- Revenue Bounded Context Initial Migration
-- Create REV schema
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'REV')
BEGIN
    EXEC('CREATE SCHEMA REV')
END
GO

-- Create ProductCategory table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[REV].[ProductCategory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [REV].[ProductCategory](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Name] [nvarchar](200) NOT NULL,
        [Description] [nvarchar](500) NULL,
        [CreationTime] [datetime2](7) NOT NULL,
        [CreatorUserId] [bigint] NULL,
        [LastModificationTime] [datetime2](7) NULL,
        [LastModifierUserId] [bigint] NULL,
        [IsDeleted] [bit] NOT NULL DEFAULT 0,
        [DeleterUserId] [bigint] NULL,
        [DeletionTime] [datetime2](7) NULL,
        CONSTRAINT [PK_ProductCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- Create Product table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[REV].[Product]') AND type in (N'U'))
BEGIN
    CREATE TABLE [REV].[Product](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Name] [nvarchar](200) NOT NULL,
        [Manufacturer] [nvarchar](200) NULL,
        [ModelNo] [nvarchar](100) NULL,
        [Description] [nvarchar](500) NULL,
        [ProductCategoryId] [int] NULL,
        [Cost] [decimal](18, 2) NOT NULL,
        [Price] [decimal](18, 2) NOT NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1,
        [CreationTime] [datetime2](7) NOT NULL,
        [CreatorUserId] [bigint] NULL,
        [LastModificationTime] [datetime2](7) NULL,
        [LastModifierUserId] [bigint] NULL,
        [IsDeleted] [bit] NOT NULL DEFAULT 0,
        [DeleterUserId] [bigint] NULL,
        [DeletionTime] [datetime2](7) NULL,
        CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Product_ProductCategory] FOREIGN KEY([ProductCategoryId]) 
            REFERENCES [REV].[ProductCategory] ([Id]) ON DELETE SET NULL
    )
END
GO

-- Create Case table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[REV].[Case]') AND type in (N'U'))
BEGIN
    CREATE TABLE [REV].[Case](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [CaseNumber] [nvarchar](50) NOT NULL,
        [ClientName] [nvarchar](200) NOT NULL,
        [Description] [nvarchar](500) NULL,
        [CaseDate] [datetime2](7) NOT NULL,
        [TotalAmount] [decimal](18, 2) NOT NULL,
        [Status] [nvarchar](50) NOT NULL,
        [Notes] [nvarchar](1000) NULL,
        [CreationTime] [datetime2](7) NOT NULL,
        [CreatorUserId] [bigint] NULL,
        [LastModificationTime] [datetime2](7) NULL,
        [LastModifierUserId] [bigint] NULL,
        [IsDeleted] [bit] NOT NULL DEFAULT 0,
        [DeleterUserId] [bigint] NULL,
        [DeletionTime] [datetime2](7) NULL,
        CONSTRAINT [PK_Case] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- Create CaseProduct table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[REV].[CaseProduct]') AND type in (N'U'))
BEGIN
    CREATE TABLE [REV].[CaseProduct](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [CaseId] [int] NOT NULL,
        [ProductId] [int] NOT NULL,
        [Quantity] [int] NOT NULL,
        [UnitPrice] [decimal](18, 2) NOT NULL,
        [Discount] [decimal](18, 2) NOT NULL DEFAULT 0,
        [TotalPrice] [decimal](18, 2) NOT NULL,
        CONSTRAINT [PK_CaseProduct] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_CaseProduct_Case] FOREIGN KEY([CaseId]) 
            REFERENCES [REV].[Case] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CaseProduct_Product] FOREIGN KEY([ProductId]) 
            REFERENCES [REV].[Product] ([Id])
    )
END
GO

-- Create indexes
CREATE NONCLUSTERED INDEX [IX_Product_ProductCategoryId] ON [REV].[Product]([ProductCategoryId])
CREATE NONCLUSTERED INDEX [IX_CaseProduct_CaseId] ON [REV].[CaseProduct]([CaseId])
CREATE NONCLUSTERED INDEX [IX_CaseProduct_ProductId] ON [REV].[CaseProduct]([ProductId])
CREATE NONCLUSTERED INDEX [IX_Case_CaseNumber] ON [REV].[Case]([CaseNumber])
CREATE NONCLUSTERED INDEX [IX_Case_ClientName] ON [REV].[Case]([ClientName])
GO

-- Insert sample data
INSERT INTO [REV].[ProductCategory] ([Name], [Description], [CreationTime])
VALUES 
    ('Electronics', 'Electronic devices and accessories', GETDATE()),
    ('Software', 'Software licenses and subscriptions', GETDATE()),
    ('Services', 'Professional and consulting services', GETDATE())
GO

INSERT INTO [REV].[Product] ([Name], [Manufacturer], [ModelNo], [Description], [ProductCategoryId], [Cost], [Price], [IsActive], [CreationTime])
VALUES 
    ('Laptop Pro', 'TechCorp', 'LP-2024', 'High-performance laptop', 1, 800.00, 1200.00, 1, GETDATE()),
    ('Office Suite', 'SoftCorp', 'OS-365', 'Complete office software package', 2, 50.00, 99.00, 1, GETDATE()),
    ('Consulting Hours', 'Internal', 'CONS-01', 'Professional consulting services', 3, 75.00, 150.00, 1, GETDATE())
GO

PRINT 'Revenue Bounded Context migration completed successfully'
GO
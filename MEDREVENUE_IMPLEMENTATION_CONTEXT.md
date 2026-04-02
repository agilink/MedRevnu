# MedRevenue Product Transactions & Quotas System - Implementation Context

**Date:** March 11, 2026
**Module:** Revenue (MedRevenue)
**Purpose:** Product transaction tracking, quota management, and comprehensive reporting for medical device procedures

---

## Table of Contents
1. [Overview](#overview)
2. [Business Requirements](#business-requirements)
3. [Database Schema](#database-schema)
4. [Architecture & File Structure](#architecture--file-structure)
5. [Implementation Details](#implementation-details)
6. [Critical Corrections](#critical-corrections)
7. [API Endpoints](#api-endpoints)
8. [Usage Guide](#usage-guide)
9. [Technical Details](#technical-details)
10. [Known Issues & Future Work](#known-issues--future-work)

---

## Overview

### Implementation Summary
This implementation adds comprehensive product transaction tracking and quota management to the MedRevenue module based on the Lafayette Quota Spreadsheet requirements. The system includes:

- **ProductSubcategory Management** - 23 procedure types across 4 main product categories
- **ProcedureTransaction Tracking** - Daily medical device procedure transactions with auto-population features
- **ProductQuota Management** - Monthly quota tracking by hospital and product category
- **Comprehensive Reports** - 4 specialized reports for revenue analysis

### Key Features
1. Auto-population of Hospital from Physician's Facility
2. Auto-fetching of Base Price based on Product and Procedure Type
3. Monthly quota tracking with variance analysis
4. 4 comprehensive reports for revenue insights
5. Advanced filtering on all list pages

---

## Business Requirements

### Source Data
**File:** Lafayette Quota SpreadsheetCopy.xlsx

**Worksheets:**
1. **ProductCategory** - 4 categories (CRM, ICD, Leadless, Other)
2. **ProductSubcategory** - 23 subcategories with procedure type classification
3. **Product** - 26 products with codes, base prices, and system flags

### Functional Requirements

#### 1. Revenue Transaction Module
- **Menu Item:** "Revenue Transaction"
- **List Page Features:**
  - Filters: Year, Month, Hospital (dropdown), Physician (dropdown)
  - Grid Columns: Hospital, Physician, Date, Product, Procedure Type, No of Cases, Base Price, Transaction Amount
- **Details Modal Features:**
  - Hospital: Auto-populated from Physician's FacilityId (hidden in UI)
  - Physician: Dropdown from Personnel table
  - Date, Product dropdown, Procedure Type (radio: NEW/DE_NOVO or GEN_CHANGE, default NEW)
  - No of Cases, Base Price (readonly), Transaction Amount
  - Validation: Transaction Amount required, No Of cases required, Procedure_Type required
  - Auto-Calculation: Base Price fetched when Physician/Procedure Type selected
  - Transaction Amount = Base Price × No of cases (user can modify)

#### 2. ProductQuota Module
- **Menu Item:** "ProductQuota"
- **List Page Features:**
  - Filters: Year (default current), Month (default current), Hospital dropdown, ProductCategory dropdown
  - Grid Columns: Year, Month, ProductCategory, OrderTransaction Value, Hospital
- **Details Modal Features:**
  - Year, Month, ProductCategory Name, OrderTransaction Value, Hospital

#### 3. Reports Module
- **Menu:** "Reports" with 4 sub-reports
  1. **Rate Chart:** Products by ProductCategory with prices for a hospital
  2. **Monthly Revenue:** Daily revenue by ProductCategory for each month
  3. **Cases by Person:** Cases per physician for last year
  4. **Transaction Amount:** By physician per product type for last year

---

## Database Schema

### Schema: REV

#### New Tables Created

**1. ProductSubcategory**
```sql
CREATE TABLE [REV].[ProductSubcategory] (
    [Id] int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [ProductCategoryId] int NOT NULL,
    [SubcategoryName] nvarchar(200) NOT NULL,
    [ProcedureType] nvarchar(20) NOT NULL,  -- 'DE_NOVO' or 'GEN_CHANGE'
    [Description] nvarchar(500) NOT NULL,
    [CreationTime] datetime2 NOT NULL,
    [CreatorUserId] bigint NULL,
    [LastModificationTime] datetime2 NULL,
    [LastModifierUserId] bigint NULL,

    CONSTRAINT [FK_ProductSubcategory_ProductCategory]
        FOREIGN KEY ([ProductCategoryId])
        REFERENCES [REV].[ProductCategory] ([Id])
);

CREATE INDEX [IX_ProductSubcategory_ProductCategoryId]
    ON [REV].[ProductSubcategory] ([ProductCategoryId]);
```

**Seeded Data:** 23 subcategories including:
- CRM: Pacemaker Single Chamber (DE_NOVO), Pacemaker Dual Chamber (DE_NOVO), etc.
- ICD: ICD Single Chamber (DE_NOVO), ICD Dual Chamber (DE_NOVO), etc.
- Leadless: Leadless AR (DE_NOVO), Leadless VR (DE_NOVO), etc.
- Battery Changes: Multiple GEN_CHANGE types

**2. ProcedureTransaction**
```sql
CREATE TABLE [REV].[ProcedureTransaction] (
    [Id] int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [ProcedureDate] datetime2 NOT NULL,
    [HospitalId] int NULL,
    [PhysicianId] int NOT NULL,
    [ProductId] int NOT NULL,
    [ProcedureType] nvarchar(20) NOT NULL,  -- 'DE_NOVO' or 'GEN_CHANGE'
    [Quantity] int NOT NULL DEFAULT 1,
    [UnitPrice] decimal(18,2) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [CreationTime] datetime2 NOT NULL,
    [CreatorUserId] bigint NULL,
    [LastModificationTime] datetime2 NULL,
    [LastModifierUserId] bigint NULL,
    [DeletionTime] datetime2 NULL,
    [DeleterUserId] bigint NULL,
    [IsDeleted] bit NOT NULL DEFAULT 0,

    CONSTRAINT [FK_ProcedureTransaction_Facility]
        FOREIGN KEY ([HospitalId])
        REFERENCES [ADM].[Facility] ([Id]),
    CONSTRAINT [FK_ProcedureTransaction_Personnel]
        FOREIGN KEY ([PhysicianId])
        REFERENCES [ADM].[Personnel] ([Id]),
    CONSTRAINT [FK_ProcedureTransaction_Product]
        FOREIGN KEY ([ProductId])
        REFERENCES [REV].[Product] ([Id])
);

CREATE INDEX [IX_ProcedureTransaction_HospitalId]
    ON [REV].[ProcedureTransaction] ([HospitalId]);
CREATE INDEX [IX_ProcedureTransaction_PhysicianId]
    ON [REV].[ProcedureTransaction] ([PhysicianId]);
CREATE INDEX [IX_ProcedureTransaction_ProductId]
    ON [REV].[ProcedureTransaction] ([ProductId]);
CREATE INDEX [IX_ProcedureTransaction_ProcedureDate]
    ON [REV].[ProcedureTransaction] ([ProcedureDate]);
```

**3. ProductQuota**
```sql
CREATE TABLE [REV].[ProductQuota] (
    [Id] int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [HospitalId] int NOT NULL,
    [ProductCategoryId] int NOT NULL,
    [ProductId] int NULL,  -- Optional for category-level quotas
    [PeriodMonth] int NOT NULL,  -- 1-12
    [PeriodYear] int NOT NULL,
    [TargetAmount] decimal(10,2) NOT NULL,
    [TargetUnits] int NULL,
    [CreationTime] datetime2 NOT NULL,
    [CreatorUserId] bigint NULL,
    [LastModificationTime] datetime2 NULL,
    [LastModifierUserId] bigint NULL,

    CONSTRAINT [FK_ProductQuota_Facility]
        FOREIGN KEY ([HospitalId])
        REFERENCES [ADM].[Facility] ([Id]),
    CONSTRAINT [FK_ProductQuota_ProductCategory]
        FOREIGN KEY ([ProductCategoryId])
        REFERENCES [REV].[ProductCategory] ([Id]),
    CONSTRAINT [FK_ProductQuota_Product]
        FOREIGN KEY ([ProductId])
        REFERENCES [REV].[Product] ([Id])
);

CREATE INDEX [IX_ProductQuota_HospitalId]
    ON [REV].[ProductQuota] ([HospitalId]);
CREATE INDEX [IX_ProductQuota_ProductCategoryId]
    ON [REV].[ProductQuota] ([ProductCategoryId]);
```

#### Modified Tables

**Product** - Added columns:
```sql
ALTER TABLE [REV].[Product] ADD [SubproductCategoryId] int NULL;
ALTER TABLE [REV].[Product] ADD [ProductCode] nvarchar(50) NULL;
ALTER TABLE [REV].[Product] ADD [IsSystem] bit NOT NULL DEFAULT 0;
ALTER TABLE [REV].[Product] ADD [BasePrice] decimal(18,2) NOT NULL DEFAULT 0;

ALTER TABLE [REV].[Product]
ADD CONSTRAINT [FK_Product_ProductSubcategory]
FOREIGN KEY ([SubproductCategoryId])
REFERENCES [REV].[ProductSubcategory] ([Id]);

CREATE INDEX [IX_Product_SubproductCategoryId]
    ON [REV].[Product] ([SubproductCategoryId]);
CREATE INDEX [IX_Product_ProductCode]
    ON [REV].[Product] ([ProductCode]);  -- NON-UNIQUE to allow NULLs
```

**ProductCategory** - Added column:
```sql
ALTER TABLE [REV].[ProductCategory] ADD [ShortDescription] nvarchar(100) NULL;
```

**Seeded Product Data:** 26 products updated with:
- Product codes (e.g., "PM-SC", "ICD-DC", "DM4500")
- Base prices ranging from $0 to $60,000
- IsSystem flags (true for complete systems with leads, false for generators only)
- Links to ProductSubcategory

---

## Architecture & File Structure

### Domain Layer Files Created

**Location:** `MedRevenue/Revenue.Domain/Entities/AggregateRoots/`

1. **ProductSubcategory.cs** - Procedure type classifications
2. **ProcedureTransaction.cs** - Medical device procedure transactions
3. **ProductQuota.cs** - Monthly quota targets

**Modified:**
- **Product.cs** - Added: SubproductCategoryId, ProductCode, IsSystem, BasePrice
- **ProductCategory.cs** - Added: ShortDescription

### Application Layer Files Created

**Location:** `MedRevenue/Revenue.Application/`

**ProcedureTransactions/**
- `IProcedureTransactionsAppService.cs`
- `ProcedureTransactionsAppService.cs`
- `Dtos/ProcedureTransactionDto.cs`
- `Dtos/CreateOrEditProcedureTransactionDto.cs`
- `Dtos/GetAllProcedureTransactionsInput.cs`
- `Dtos/GetProcedureTransactionForViewDto.cs`
- `Dtos/GetProcedureTransactionForEditOutput.cs`

**ProductQuotas/**
- `IProductQuotasAppService.cs`
- `ProductQuotasAppService.cs`
- `Dtos/ProductQuotaDto.cs`
- `Dtos/CreateOrEditProductQuotaDto.cs`
- `Dtos/GetAllProductQuotasInput.cs`
- `Dtos/GetProductQuotaForViewDto.cs`
- `Dtos/GetProductQuotaForEditOutput.cs`

**Reports/**
- `IReportsAppService.cs`
- `ReportsAppService.cs`
- `Dtos/RateChartReportDto.cs`
- `Dtos/MonthlyRevenueReportDto.cs`
- `Dtos/CasesByPersonReportDto.cs`
- `Dtos/TransactionAmountReportDto.cs`
- `Dtos/RateChartReportInput.cs`
- `Dtos/MonthlyRevenueReportInput.cs`
- `Dtos/CasesByPersonReportInput.cs`
- `Dtos/TransactionAmountReportInput.cs`

**Products/**
- `IProductsAppService.cs`
- `ProductsAppService.cs`
- `Dtos/ProductDto.cs`
- `Dtos/CreateOrEditProductDto.cs`

**Modified:**
- `RevenueDtoMapper.cs` - Added mappings for new entities (CRITICAL: Uses `Hospital.FacilityName`)

### Web Layer Files Created

**Controllers:**
`MedRevenue/Revenue.Web/Areas/Revenue/Controllers/`
- `ProcedureTransactionsController.cs` - AJAX endpoints and modal views
- `ProductQuotasController.cs` - AJAX endpoints and modal views
- `ReportsController.cs` - 4 report views and data endpoints

**Views:**
`MedRevenue/Revenue.Web/Areas/Revenue/Views/`

**ProcedureTransactions/**
- `Index.cshtml` - List page with filters
- `_CreateOrEditModal.cshtml` - Transaction entry form

**ProductQuotas/**
- `Index.cshtml` - List page with filters
- `_CreateOrEditModal.cshtml` - Quota entry form

**Reports/**
- `RateChart.cshtml` - Product pricing by category
- `MonthlyRevenue.cshtml` - Daily revenue breakdown
- `CasesByPerson.cshtml` - Physician performance
- `TransactionAmount.cshtml` - Physician/product analysis

**JavaScript:**
`MedRevenue/Revenue.Web/wwwroot/view-resources/Areas/Revenue/Views/`

**ProcedureTransactions/**
- `Index.js` - DataTable configuration with filters
- `_CreateOrEditModal.js` - Auto-population logic for Hospital, BasePrice, TotalAmount

**ProductQuotas/**
- `Index.js` - DataTable configuration with filters
- `_CreateOrEditModal.js` - Form handling

**Reports/**
- `RateChart.js` - AJAX data loading
- `MonthlyRevenue.js` - AJAX data loading
- `CasesByPerson.js` - AJAX data loading with year selector
- `TransactionAmount.js` - AJAX data loading with filters

### Navigation Menu Structure

**File:** `src/ATI.Web.Mvc/Areas/Core/Startup/CoreNavigationProvider.cs`

**Added Menu Items:**
```
Revenue (parent)
├── Cases
├── Revenue Transactions (NEW)
├── Product Quotas (NEW)
└── Reports (NEW)
    ├── Rate Chart
    ├── Monthly Revenue
    ├── Cases by Person
    └── Transaction Amount
```

---

## Implementation Details

### Key Business Logic

#### 1. Auto-Population in ProcedureTransactions

**Hospital Auto-Population:**
```csharp
// In ProcedureTransactionsAppService.cs Create method
var physician = await _personnelRepository.GetAsync(input.PhysicianId);
input.HospitalId = physician.FacilityId;  // Auto-set from physician's facility
```

**Base Price Auto-Fetch:**
```csharp
public async Task<decimal> GetProductBasePrice(int productId, string procedureType)
{
    var product = await _productRepository.GetAsync(productId);
    return product.BasePrice;  // Same price regardless of procedure type
}
```

**Total Amount Calculation:**
```csharp
// Auto-calculate if not provided or if user hasn't modified it
if (input.TotalAmount == 0 || input.TotalAmount == input.UnitPrice * input.Quantity)
{
    input.TotalAmount = input.UnitPrice * input.Quantity;
}
// Otherwise, use user-provided override
```

#### 2. ProductQuota Filtering

**CreateFilteredQuery Logic:**
```csharp
private IQueryable<ProductQuota> CreateFilteredQuery(GetAllProductQuotasInput input)
{
    var query = _productQuotaRepository.GetAll();

    if (input.YearFilter.HasValue)
        query = query.Where(pq => pq.PeriodYear == input.YearFilter.Value);

    if (input.MonthFilter.HasValue)
        query = query.Where(pq => pq.PeriodMonth == input.MonthFilter.Value);

    if (input.HospitalIdFilter.HasValue)
        query = query.Where(pq => pq.HospitalId == input.HospitalIdFilter.Value);

    if (input.ProductCategoryIdFilter.HasValue)
        query = query.Where(pq => pq.ProductCategoryId == input.ProductCategoryIdFilter.Value);

    if (!string.IsNullOrWhiteSpace(input.Filter))
    {
        query = query.Where(pq =>
            pq.Hospital.FacilityName.Contains(input.Filter) ||
            pq.ProductCategory.Name.Contains(input.Filter) ||
            (pq.Product != null && pq.Product.Name.Contains(input.Filter))
        );
    }

    return query;
}
```

#### 3. Reports Implementation

**Rate Chart Report:**
```csharp
public async Task<List<RateChartReportDto>> GetRateChartReport(RateChartReportInput input)
{
    var products = await _productRepository
        .GetAll()
        .Include(p => p.ProductCategory)
        .Where(p => p.IsActive && p.ProductCategory != null)
        .OrderBy(p => p.ProductCategory.Name)
        .ThenBy(p => p.Name)
        .Select(p => new RateChartReportDto
        {
            ProductCategoryName = p.ProductCategory.Name,
            ProductCode = p.ProductCode,
            ProductName = p.Name,
            BasePrice = p.BasePrice,
            IsSystem = p.IsSystem
        })
        .ToListAsync();
    return products;
}
```

**Monthly Revenue Report:**
```csharp
public async Task<List<MonthlyRevenueReportDto>> GetMonthlyRevenueReport(MonthlyRevenueReportInput input)
{
    var startDate = new DateTime(input.Year, input.Month, 1);
    var endDate = startDate.AddMonths(1).AddDays(-1);

    var query = _procedureTransactionRepository
        .GetAll()
        .Include(pt => pt.Product)
            .ThenInclude(p => p.ProductCategory)
        .Where(pt => pt.ProcedureDate >= startDate && pt.ProcedureDate <= endDate);

    if (input.HospitalId.HasValue)
        query = query.Where(pt => pt.HospitalId == input.HospitalId.Value);

    var report = await query
        .GroupBy(pt => new
        {
            Date = pt.ProcedureDate.Date,
            ProductCategoryName = pt.Product.ProductCategory.Name
        })
        .Select(g => new MonthlyRevenueReportDto
        {
            Date = g.Key.Date,
            ProductCategoryName = g.Key.ProductCategoryName,
            TotalRevenue = g.Sum(pt => pt.TotalAmount),
            TransactionCount = g.Count()
        })
        .OrderBy(r => r.Date)
        .ThenBy(r => r.ProductCategoryName)
        .ToListAsync();
    return report;
}
```

**Cases by Person Report:**
```csharp
public async Task<List<CasesByPersonReportDto>> GetCasesByPersonReport(CasesByPersonReportInput input)
{
    var query = _procedureTransactionRepository
        .GetAll()
        .Include(pt => pt.Physician)
            .ThenInclude(p => p.Facility)
        .Where(pt => pt.ProcedureDate.Year == input.Year);

    if (input.HospitalId.HasValue)
        query = query.Where(pt => pt.HospitalId == input.HospitalId.Value);

    if (input.PhysicianId.HasValue)
        query = query.Where(pt => pt.PhysicianId == input.PhysicianId.Value);

    var report = await query
        .GroupBy(pt => new
        {
            PhysicianId = pt.PhysicianId,
            PhysicianFirstName = pt.Physician.FIRST_NAME,
            PhysicianLastName = pt.Physician.LAST_NAME,
            HospitalName = pt.Hospital != null ? pt.Hospital.FacilityName : ""
        })
        .Select(g => new CasesByPersonReportDto
        {
            PhysicianName = (g.Key.PhysicianFirstName + " " + g.Key.PhysicianLastName).Trim(),
            HospitalName = g.Key.HospitalName,
            TotalCases = g.Sum(pt => pt.Quantity),
            DeNovoCases = g.Where(pt => pt.ProcedureType == "DE_NOVO").Sum(pt => pt.Quantity),
            GenChangeCases = g.Where(pt => pt.ProcedureType == "GEN_CHANGE").Sum(pt => pt.Quantity),
            TotalRevenue = g.Sum(pt => pt.TotalAmount)
        })
        .OrderByDescending(r => r.TotalCases)
        .ToListAsync();
    return report;
}
```

**Transaction Amount Report:**
```csharp
public async Task<List<TransactionAmountReportDto>> GetTransactionAmountReport(TransactionAmountReportInput input)
{
    var query = _procedureTransactionRepository
        .GetAll()
        .Include(pt => pt.Physician)
        .Include(pt => pt.Product)
            .ThenInclude(p => p.ProductCategory)
        .Where(pt => pt.ProcedureDate.Year == input.Year);

    if (input.PhysicianId.HasValue)
        query = query.Where(pt => pt.PhysicianId == input.PhysicianId.Value);

    if (input.ProductCategoryId.HasValue)
        query = query.Where(pt => pt.Product.ProductCategoryId == input.ProductCategoryId.Value);

    var report = await query
        .GroupBy(pt => new
        {
            PhysicianName = (pt.Physician.FIRST_NAME + " " + pt.Physician.LAST_NAME).Trim(),
            ProductCategoryName = pt.Product.ProductCategory.Name,
            ProcedureType = pt.ProcedureType
        })
        .Select(g => new TransactionAmountReportDto
        {
            PhysicianName = g.Key.PhysicianName,
            ProductCategoryName = g.Key.ProductCategoryName,
            ProcedureType = g.Key.ProcedureType,
            TotalAmount = g.Sum(pt => pt.TotalAmount),
            TotalQuantity = g.Sum(pt => pt.Quantity)
        })
        .OrderBy(r => r.PhysicianName)
        .ThenBy(r => r.ProductCategoryName)
        .ToListAsync();
    return report;
}
```

---

## Critical Corrections

### Facility Property Name Issue (CORRECTED)

**Problem Identified:** Initial implementation incorrectly used `Hospital.Name` instead of `Hospital.FacilityName`

**Root Cause:** The Facility entity uses `FacilityName` as its property name, not `Name`

**Files Corrected (7 total):**

1. **ProcedureTransactionsAppService.cs** - Line 182, 222, 269
   ```csharp
   // BEFORE (incorrect):
   HospitalName = entity.Hospital?.Name ?? ""

   // AFTER (correct):
   HospitalName = entity.Hospital?.FacilityName ?? ""
   ```

2. **ProductQuotasAppService.cs** - Lines 67, 99, 183, 221, 269
   ```csharp
   // BEFORE (incorrect):
   HospitalName = entity.Hospital?.Name ?? ""
   query.Where(pq => pq.Hospital.Name.Contains(input.Filter))

   // AFTER (correct):
   HospitalName = entity.Hospital?.FacilityName ?? ""
   query.Where(pq => pq.Hospital.FacilityName.Contains(input.Filter))
   ```

3. **ReportsAppService.cs** - Line 138
   ```csharp
   // BEFORE (incorrect):
   HospitalName = pt.Hospital != null ? pt.Hospital.Name : ""

   // AFTER (correct):
   HospitalName = pt.Hospital != null ? pt.Hospital.FacilityName : ""
   ```

4. **RevenueDtoMapper.cs** - Lines 50, 67
   ```csharp
   // BEFORE (incorrect):
   .ForMember(dto => dto.HospitalName,
       opt => opt.MapFrom(src => src.Hospital != null ? src.Hospital.Name : string.Empty))

   // AFTER (correct):
   .ForMember(dto => dto.HospitalName,
       opt => opt.MapFrom(src => src.Hospital != null ? src.Hospital.FacilityName : string.Empty))
   ```

5. **ProcedureTransactionsController.cs** - Lines 122, 136
   ```csharp
   // BEFORE (incorrect):
   facilityName = facility?.Name ?? ""
   hospitals.OrderBy(h => h.Name)

   // AFTER (correct):
   facilityName = facility?.FacilityName ?? ""
   hospitals.OrderBy(h => h.FacilityName)
   ```

6. **ProductQuotasController.cs** - Lines 93, 95
   ```csharp
   // BEFORE (incorrect):
   hospitals.OrderBy(h => h.Name), "FacilityName"

   // AFTER (correct):
   hospitals.OrderBy(h => h.FacilityName), "FacilityName"
   ```

7. **ReportsController.cs** - Lines 124, 126
   ```csharp
   // BEFORE (incorrect):
   hospitals.OrderBy(h => h.Name), "FacilityName"

   // AFTER (correct):
   hospitals.OrderBy(h => h.FacilityName), "FacilityName"
   ```

**Verification:**
```csharp
// Facility entity confirmed:
public partial class Facility : FullAuditedEntity
{
    public string FacilityName { get; set; }  // ← Correct property name
    // NOT: public string Name { get; set; }
}
```

**Impact:** This correction prevents runtime null reference errors and ensures hospital names display correctly throughout the application.

---

## API Endpoints

### ProcedureTransactions Endpoints
```
GET  /Revenue/ProcedureTransactions
     Returns: Index view

POST /Revenue/ProcedureTransactions/GetAll
     Body: GetAllProcedureTransactionsInput
     Returns: PagedResultDto<ProcedureTransactionDto>

GET  /Revenue/ProcedureTransactions/CreateOrEditModal?id={id}
     Returns: Partial view with form

POST /Revenue/ProcedureTransactions/CreateOrEdit
     Body: CreateOrEditProcedureTransactionDto
     Returns: JSON { success, result }

POST /Revenue/ProcedureTransactions/Delete
     Body: { id: number }
     Returns: JSON { success }

POST /Revenue/ProcedureTransactions/GetProductBasePrice
     Body: { productId: number, procedureType: string }
     Returns: JSON { success, basePrice }

POST /Revenue/ProcedureTransactions/GetPhysicianFacility
     Body: { physicianId: number }
     Returns: JSON { success, facilityId, facilityName }
```

### ProductQuotas Endpoints
```
GET  /Revenue/ProductQuotas
     Returns: Index view

POST /Revenue/ProductQuotas/GetAll
     Body: GetAllProductQuotasInput
     Returns: PagedResultDto<ProductQuotaDto>

GET  /Revenue/ProductQuotas/CreateOrEditModal?id={id}
     Returns: Partial view with form

POST /Revenue/ProductQuotas/CreateOrEdit
     Body: CreateOrEditProductQuotaDto
     Returns: JSON { success, result }

POST /Revenue/ProductQuotas/Delete
     Body: { id: number }
     Returns: JSON { success }
```

### Reports Endpoints
```
GET  /Revenue/Reports/RateChart
     Returns: View

POST /Revenue/Reports/GetRateChartData
     Body: { hospitalId: number }
     Returns: JSON { success, data: RateChartReportDto[] }

GET  /Revenue/Reports/MonthlyRevenue
     Returns: View

POST /Revenue/Reports/GetMonthlyRevenueData
     Body: { year: number, month: number, hospitalId?: number }
     Returns: JSON { success, data: MonthlyRevenueReportDto[] }

GET  /Revenue/Reports/CasesByPerson
     Returns: View

POST /Revenue/Reports/GetCasesByPersonData
     Body: { year: number, hospitalId?: number, physicianId?: number }
     Returns: JSON { success, data: CasesByPersonReportDto[] }

GET  /Revenue/Reports/TransactionAmount
     Returns: View

POST /Revenue/Reports/GetTransactionAmountData
     Body: { year: number, physicianId?: number, productCategoryId?: number }
     Returns: JSON { success, data: TransactionAmountReportDto[] }
```

---

## Usage Guide

### Creating a Procedure Transaction

1. Navigate to **Revenue → Revenue Transactions**
2. Click **"Create New"** button
3. **Auto-Population Workflow:**
   - Select **Physician** from dropdown
   - Hospital field auto-populates (hidden) from Physician's Facility
   - Select **Product** from dropdown
   - Choose **Procedure Type** radio button (NEW or GEN CHANGE)
   - Base Price automatically fetches from Product
4. **Manual Entry:**
   - Enter **Procedure Date**
   - Enter **No of Cases** (default 1)
   - Verify **Base Price** (readonly)
   - Verify **Transaction Amount** (auto-calculated, but editable)
5. Click **Save**

### Managing Product Quotas

1. Navigate to **Revenue → Product Quotas**
2. Use filters to find specific quotas:
   - Year (default: current year)
   - Month (default: current month)
   - Hospital
   - Product Category
3. Click **"Create New"** to add quota
4. Fill in:
   - Year, Month
   - Hospital
   - Product Category
   - (Optional) Specific Product
   - Target Amount (revenue goal)
   - (Optional) Target Units
5. Click **Save**

### Viewing Reports

#### 1. Rate Chart
- Navigate to **Revenue → Reports → Rate Chart**
- Select Hospital from dropdown
- View products organized by category with pricing
- Shows Product Code, Name, Base Price, and System Type

#### 2. Monthly Revenue
- Navigate to **Revenue → Reports → Monthly Revenue**
- Select Year and Month
- (Optional) Filter by Hospital
- View daily revenue breakdown by product category

#### 3. Cases by Person
- Navigate to **Revenue → Reports → Cases by Person**
- Select Year (default: last year)
- (Optional) Filter by Hospital or Physician
- View physician performance with case counts and revenue

#### 4. Transaction Amount
- Navigate to **Revenue → Reports → Transaction Amount**
- Select Year (default: last year)
- (Optional) Filter by Physician or Product Category
- View revenue by physician and product type

---

## Technical Details

### Technology Stack
- **.NET 8.0**
- **ASP.NET Core MVC** (server-side rendering)
- **Entity Framework Core 8.0.8**
- **ABP Framework 9.4.2**
- **SQL Server** (schemas: REV, ADM)
- **jQuery 3.7.1 + AJAX**
- **DataTables 2.1.6**
- **Bootstrap 5.3.3**
- **AutoMapper**

### Design Patterns
1. **Repository Pattern** - `IRepository<T, TPrimaryKey>`
2. **Unit of Work** - ABP's automatic transaction management
3. **Domain-Driven Design** - Aggregate roots, entities, value objects
4. **DTO Pattern** - Data transfer objects for API communication
5. **Dependency Injection** - Constructor injection throughout

### Performance Optimizations
1. **Eager Loading** - `.Include()` for related entities
2. **Indexed Columns** - Foreign keys, date fields
3. **Pagination** - All list queries use `PageBy()`
4. **Filtered Queries** - Database-level filtering with LINQ

### Security
- **SQL Injection Prevention** - Parameterized queries via EF Core
- **XSS Prevention** - Razor auto-encoding
- **CSRF Protection** - Anti-forgery tokens on forms
- **Authorization** - Permission-based access control (to be implemented)

---

## Known Issues & Future Work

### Known Issues
1. **No Permission Gates** - Controllers need `[AbpAuthorize]` attributes
2. **No Client-Side Validation** - jQuery Validation rules needed
3. **No Error Handling UI** - Generic AJAX error messages

### Future Enhancements

#### High Priority
1. **Add Permission Definitions**
   ```csharp
   public const string Pages_Revenue_ProcedureTransactions = "Pages.Revenue.ProcedureTransactions";
   public const string Pages_Revenue_ProductQuotas = "Pages.Revenue.ProductQuotas";
   public const string Pages_Revenue_Reports = "Pages.Revenue.Reports";
   ```

2. **Implement Bulk Import**
   - CSV/Excel import for transactions
   - Validation and error reporting

3. **Add Export to Excel**
   - Export reports to Excel format
   - Include charts and formatting

#### Medium Priority
4. **Dashboard Widgets**
   - Current month quota vs. actual
   - Top performing physicians
   - Revenue trend chart

5. **Email Notifications**
   - Monthly quota achievement alerts
   - Daily transaction summaries

6. **Audit Logging**
   - Track all transaction changes
   - Log quota modifications

#### Low Priority
7. **Advanced Analytics**
   - Forecasting based on trends
   - Seasonal analysis
   - Product mix optimization

8. **Mobile Optimization**
   - Responsive design improvements
   - Touch-friendly interfaces

---

## Migration History

### Applied Migrations
1. **20260306150000_AddProcedureTypesIncrementally**
   - Added ProcedureType and ProcedureQuota tables (previous implementation)

2. **20260311000000_AddProductTransactionEntities**
   - Added ProductSubcategory table
   - Added ProcedureTransaction table
   - Added ProductQuota table
   - Modified Product table (added SubproductCategoryId, ProductCode, IsSystem, BasePrice)
   - Modified ProductCategory table (added ShortDescription)

### Data Seeding
- **ProductCategory:** 4 categories (CRM, ICD, Leadless, Other)
- **ProductSubcategory:** 23 subcategories with procedure types
- **Product:** 26 products with codes, prices, and system flags

---

## Rollback Instructions

### Database Rollback
```sql
-- Drop foreign keys
ALTER TABLE [REV].[ProcedureTransaction] DROP CONSTRAINT [FK_ProcedureTransaction_Facility];
ALTER TABLE [REV].[ProcedureTransaction] DROP CONSTRAINT [FK_ProcedureTransaction_Personnel];
ALTER TABLE [REV].[ProcedureTransaction] DROP CONSTRAINT [FK_ProcedureTransaction_Product];
ALTER TABLE [REV].[ProductQuota] DROP CONSTRAINT [FK_ProductQuota_Facility];
ALTER TABLE [REV].[ProductQuota] DROP CONSTRAINT [FK_ProductQuota_ProductCategory];
ALTER TABLE [REV].[ProductQuota] DROP CONSTRAINT [FK_ProductQuota_Product];
ALTER TABLE [REV].[Product] DROP CONSTRAINT [FK_Product_ProductSubcategory];

-- Drop new tables
DROP TABLE [REV].[ProcedureTransaction];
DROP TABLE [REV].[ProductQuota];
DROP TABLE [REV].[ProductSubcategory];

-- Revert Product table changes
ALTER TABLE [REV].[Product] DROP COLUMN [SubproductCategoryId];
ALTER TABLE [REV].[Product] DROP COLUMN [ProductCode];
ALTER TABLE [REV].[Product] DROP COLUMN [IsSystem];
ALTER TABLE [REV].[Product] DROP COLUMN [BasePrice];

-- Revert ProductCategory table changes
ALTER TABLE [REV].[ProductCategory] DROP COLUMN [ShortDescription];

-- Delete migration record
DELETE FROM [dbo].[__EFMigrationsHistory]
WHERE [MigrationId] = '20260311000000_AddProductTransactionEntities';
```

### Code Rollback
```bash
# Delete new folders
rm -rf MedRevenue/Revenue.Application/ProcedureTransactions/
rm -rf MedRevenue/Revenue.Application/ProductQuotas/
rm -rf MedRevenue/Revenue.Application/Reports/
rm -rf MedRevenue/Revenue.Application/Products/
rm -rf MedRevenue/Revenue.Web/Areas/Revenue/Controllers/ProcedureTransactionsController.cs
rm -rf MedRevenue/Revenue.Web/Areas/Revenue/Controllers/ProductQuotasController.cs
rm -rf MedRevenue/Revenue.Web/Areas/Revenue/Controllers/ReportsController.cs
rm -rf MedRevenue/Revenue.Web/Areas/Revenue/Views/ProcedureTransactions/
rm -rf MedRevenue/Revenue.Web/Areas/Revenue/Views/ProductQuotas/
rm -rf MedRevenue/Revenue.Web/Areas/Revenue/Views/Reports/
rm -rf MedRevenue/Revenue.Web/wwwroot/view-resources/Areas/Revenue/Views/ProcedureTransactions/
rm -rf MedRevenue/Revenue.Web/wwwroot/view-resources/Areas/Revenue/Views/ProductQuotas/
rm -rf MedRevenue/Revenue.Web/wwwroot/view-resources/Areas/Revenue/Views/Reports/

# Revert modified files using git
git checkout HEAD -- MedRevenue/Revenue.Domain/Entities/AggregateRoots/Product.cs
git checkout HEAD -- MedRevenue/Revenue.Domain/Entities/AggregateRoots/ProductCategory.cs
git checkout HEAD -- MedRevenue/Revenue.Application/RevenueDtoMapper.cs
git checkout HEAD -- src/ATI.Web.Mvc/Areas/Core/Startup/CoreNavigationProvider.cs
```

---

## Contact & Support

**Implementation Date:** March 11, 2026
**Developer:** Claude Code (Anthropic)
**Framework:** ASP.NET Zero 13.4.0 / ABP 9.4.2
**Database:** SQL Server (MedRevnu database at 10.21.30.169:1436)

**Source Document:** Lafayette Quota SpreadsheetCopy.xlsx

For technical questions:
- ABP Framework: https://docs.abp.io/
- ASP.NET Zero: https://docs.aspnetzero.com/
- Entity Framework Core: https://docs.microsoft.com/en-us/ef/core/

---

**End of Implementation Context Document**

using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using ATI.Admin.Domain.Entities;
using ATI.Revenue.Application.Reports.Dtos;
using ATI.Revenue.Domain.Entities;
using ATI.Revenue.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Reports
{
    public class ReportsAppService : ApplicationService, IReportsAppService
    {
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<ProductCategory, int> _productCategoryRepository;
        private readonly IRepository<ProcedureTransaction, int> _procedureTransactionRepository;
        private readonly IRepository<Personnel, int> _personnelRepository;
        private readonly IRepository<Facility, int> _facilityRepository;
        private readonly IRepository<ProductQuota, int> _productQuotaRepository;
        private readonly IRepository<HospitalProductPrice, int> _hospitalProductPriceRepository;

        public ReportsAppService(
            IRepository<Product, int> productRepository,
            IRepository<ProductCategory, int> productCategoryRepository,
            IRepository<ProcedureTransaction, int> procedureTransactionRepository,
            IRepository<Personnel, int> personnelRepository,
            IRepository<Facility, int> facilityRepository,
            IRepository<ProductQuota, int> productQuotaRepository,
            IRepository<HospitalProductPrice, int> hospitalProductPriceRepository)
        {
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _procedureTransactionRepository = procedureTransactionRepository;
            _personnelRepository = personnelRepository;
            _facilityRepository = facilityRepository;
            _productQuotaRepository = productQuotaRepository;
            _hospitalProductPriceRepository = hospitalProductPriceRepository;
        }

        /// <summary>
        /// Report 1: Rate Chart - Products by ProductCategory with prices for a hospital
        /// </summary>
        public async Task<List<RateChartReportDto>> GetRateChartReport(RateChartReportInput input)
        {
            // The hospital filter used to be ignored entirely: the report always returned
            // Product.BasePrice, so picking a hospital changed nothing. A hospital's
            // contracted prices are what a case is actually billed at, so those are shown
            // when a hospital is chosen, with the base price alongside for comparison.
            var asOf = Abp.Timing.Clock.Now;
            var year = asOf.Year;

            var products = await _productRepository.GetAll()
                .Where(p => p.IsActive && p.ProductCategory != null)
                .Select(p => new
                {
                    p.Id,
                    ProductCategoryName = p.ProductCategory.Name,
                    p.ProductCode,
                    ProductName = p.Name,
                    p.BasePrice,
                    p.IsSystem
                })
                .ToListAsync();

            var contracted = input.HospitalId.HasValue
                ? await _hospitalProductPriceRepository.GetAll()
                    .Where(hpp => hpp.HospitalId == input.HospitalId.Value
                                  && hpp.IsActive
                                  && (hpp.EffectiveDate == null || hpp.EffectiveDate <= asOf))
                    .OrderBy(hpp => hpp.ProductId)
                    .ThenByDescending(hpp => hpp.EffectiveDate)
                    .Select(hpp => new { hpp.ProductId, hpp.UnitPrice })
                    .ToListAsync()
                : new List<dynamic>().Select(x => new { ProductId = 0, UnitPrice = 0m }).ToList();

            // Newest in-effect price per product, matching how a case is priced.
            var priceByProduct = contracted
                .GroupBy(c => c.ProductId)
                .ToDictionary(g => g.Key, g => g.First().UnitPrice);

            // Volume comes from the device lines, optionally narrowed to the hospital.
            var volume = await _procedureTransactionRepository.GetAll()
                .Where(pt => pt.ProcedureDate.Year == year)
                .WhereIf(input.HospitalId.HasValue, pt => pt.HospitalId == input.HospitalId.Value)
                .SelectMany(pt => pt.Products.Select(l => new { CaseId = pt.Id, l.ProductId, l.Quantity }))
                .ToListAsync();

            var volumeByProduct = volume
                .GroupBy(v => v.ProductId)
                .ToDictionary(g => g.Key, g => new
                {
                    Units = g.Sum(v => v.Quantity),
                    Cases = g.Select(v => v.CaseId).Distinct().Count()
                });

            return products
                .Select(p =>
                {
                    var hasContracted = priceByProduct.TryGetValue(p.Id, out var contractedPrice);
                    volumeByProduct.TryGetValue(p.Id, out var sold);

                    return new RateChartReportDto
                    {
                        ProductCategoryName = p.ProductCategoryName,
                        ProductCode = p.ProductCode,
                        ProductName = p.ProductName,
                        BasePrice = p.BasePrice,
                        ContractedPrice = hasContracted ? contractedPrice : (decimal?)null,
                        EffectivePrice = hasContracted ? contractedPrice : p.BasePrice,
                        UnitsSoldThisYear = sold?.Units ?? 0,
                        CasesThisYear = sold?.Cases ?? 0,
                        IsSystem = p.IsSystem
                    };
                })
                .OrderBy(r => r.ProductCategoryName)
                .ThenBy(r => r.ProductName)
                .ToList();
        }

        /// <summary>
        /// Report 2: Monthly Revenue - Daily revenue by ProductCategory for each month
        /// </summary>
        public async Task<List<MonthlyRevenueReportDto>> GetMonthlyRevenueReport(MonthlyRevenueReportInput input)
        {
            // Filters are optional; fall back to the current month rather than failing.
            var year = input.Year ?? Abp.Timing.Clock.Now.Year;
            var month = input.Month ?? Abp.Timing.Clock.Now.Month;

            // Revenue per category comes from the device lines: one case can involve
            // devices from more than one category, so the case total cannot be attributed
            // to a single one.
            var report = await _procedureTransactionRepository.GetAll()
                .WhereIf(input.HospitalId.HasValue, pt => pt.HospitalId == input.HospitalId.Value)
                .Where(pt => pt.ProcedureDate.Year == year && pt.ProcedureDate.Month == month)
                .SelectMany(pt => pt.Products.Select(l => new
                {
                    CaseId = pt.Id,
                    Date = pt.ProcedureDate.Date,
                    CategoryName = l.Product != null && l.Product.ProductCategory != null
                        ? l.Product.ProductCategory.Name
                        : null,
                    l.Quantity,
                    l.LineTotal
                }))
                .GroupBy(x => new { x.Date, x.CategoryName })
                .Select(g => new MonthlyRevenueReportDto
                {
                    ProcedureDate = g.Key.Date,
                    ProductCategoryName = g.Key.CategoryName ?? "(Uncategorised)",
                    DailyRevenue = g.Sum(x => x.LineTotal),
                    // Cases, not device lines.
                    TransactionCount = g.Select(x => x.CaseId).Distinct().Count(),
                    TotalUnits = g.Sum(x => x.Quantity)
                })
                .OrderBy(r => r.ProcedureDate)
                .ThenBy(r => r.ProductCategoryName)
                .ToListAsync();

            return report;
        }

        /// <summary>
        /// Report 3: Cases by Person - Cases per physician for specified year
        /// </summary>
        public async Task<List<CasesByPersonReportDto>> GetCasesByPersonReport(CasesByPersonReportInput input)
        {
            var year = input.Year ?? Abp.Timing.Clock.Now.Year;

            var query = _procedureTransactionRepository
                .GetAll()
                .Include(pt => pt.Physician)
                    .ThenInclude(p => p.Facility)
                .Where(pt => pt.ProcedureDate.Year == year);

            if (input.HospitalId.HasValue)
            {
                query = query.Where(pt => pt.HospitalId == input.HospitalId.Value);
            }

            if (input.PhysicianId.HasValue)
            {
                query = query.Where(pt => pt.PhysicianId == input.PhysicianId.Value);
            }

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
                    // A case is one row, so counts are row counts. Summing device
                    // quantities would report a three-device procedure as three cases.
                    TotalCases = g.Count(),
                    DeNovoCases = g.Count(pt => pt.ImplantType == ImplantType.DeNovo),
                    GenChangeCases = g.Count(pt => pt.ImplantType == ImplantType.GenChange),
                    TotalRevenue = g.Sum(pt => pt.TotalAmount)
                })
                .OrderByDescending(r => r.TotalCases)
                .ToListAsync();

            return report;
        }

        /// <summary>
        /// Report 4: Transaction Amount - By physician per product type for specified year
        /// </summary>
        public async Task<List<TransactionAmountReportDto>> GetTransactionAmountReport(TransactionAmountReportInput input)
        {
            var year = input.Year ?? Abp.Timing.Clock.Now.Year;

            // Per-category figures come from the device lines, since a case can involve
            // devices from several categories. Case counts are distinct cases, and the
            // average is per case rather than per line.
            var rows = await _procedureTransactionRepository.GetAll()
                .Where(pt => pt.ProcedureDate.Year == year)
                .WhereIf(input.PhysicianId.HasValue, pt => pt.PhysicianId == input.PhysicianId.Value)
                .SelectMany(pt => pt.Products.Select(l => new
                {
                    CaseId = pt.Id,
                    PhysicianFirstName = pt.Physician.FIRST_NAME,
                    PhysicianLastName = pt.Physician.LAST_NAME,
                    pt.ImplantType,
                    ProductCategoryId = l.Product != null ? l.Product.ProductCategoryId : null,
                    CategoryName = l.Product != null && l.Product.ProductCategory != null
                        ? l.Product.ProductCategory.Name
                        : null,
                    l.Quantity,
                    l.LineTotal
                }))
                .WhereIf(input.ProductCategoryId.HasValue,
                    x => x.ProductCategoryId == input.ProductCategoryId.Value)
                .ToListAsync();

            return rows
                .GroupBy(x => new
                {
                    x.PhysicianFirstName,
                    x.PhysicianLastName,
                    x.CategoryName,
                    x.ImplantType
                })
                .Select(g =>
                {
                    var caseCount = g.Select(x => x.CaseId).Distinct().Count();
                    var total = g.Sum(x => x.LineTotal);

                    return new TransactionAmountReportDto
                    {
                        PhysicianName = ((g.Key.PhysicianFirstName ?? "") + " " + (g.Key.PhysicianLastName ?? "")).Trim(),
                        ProductCategoryName = g.Key.CategoryName ?? "(Uncategorised)",
                        ImplantType = g.Key.ImplantType,
                        TotalCases = caseCount,
                        TotalUnits = g.Sum(x => x.Quantity),
                        TotalAmount = total,
                        AverageAmount = caseCount > 0 ? total / caseCount : 0m
                    };
                })
                .OrderBy(r => r.PhysicianName)
                .ThenBy(r => r.ProductCategoryName)
                .ThenBy(r => r.ImplantType)
                .ToList();
        }

        /// <summary>
        /// Report 5: Quarterly rollup - three months of targets against actuals.
        /// </summary>
        /// <remarks>
        /// The workbook's "Quarters" tab, which the client sums by hand: Total Sold,
        /// Total Plan and Percent to Plan per product category. Categories with revenue
        /// but no target are still listed, so revenue cannot disappear from the rollup
        /// just because nobody set a plan for it.
        /// </remarks>
        public async Task<List<QuarterlyRollupReportDto>> GetQuarterlyRollupReport(QuarterlyRollupReportInput input)
        {
            var year = input.Year ?? Abp.Timing.Clock.Now.Year;

            var quarters = input.Quarter.HasValue
                ? new[] { input.Quarter.Value }
                : new[] { 1, 2, 3, 4 };

            var months = quarters.ToDictionary(q => q, q => new[] { (q - 1) * 3 + 1, (q - 1) * 3 + 2, (q - 1) * 3 + 3 });
            var allMonths = months.Values.SelectMany(m => m).ToList();

            var plans = await _productQuotaRepository.GetAll()
                .Where(q => q.PeriodYear == year && allMonths.Contains(q.PeriodMonth))
                .WhereIf(input.HospitalId.HasValue, q => q.HospitalId == input.HospitalId.Value)
                .Select(q => new
                {
                    q.PeriodMonth,
                    q.HospitalId,
                    HospitalName = q.Hospital != null ? q.Hospital.FacilityName : null,
                    q.ProductCategoryId,
                    ProductCategoryName = q.ProductCategory != null ? q.ProductCategory.Name : null,
                    q.TargetAmount
                })
                .ToListAsync();

            // Quotas are per product category, so actuals come from the device lines: one
            // case can involve devices from more than one category.
            var actuals = await _procedureTransactionRepository.GetAll()
                .Where(pt => pt.ProcedureDate.Year == year && allMonths.Contains(pt.ProcedureDate.Month))
                .WhereIf(input.HospitalId.HasValue, pt => pt.HospitalId == input.HospitalId.Value)
                .SelectMany(pt => pt.Products.Select(l => new
                {
                    CaseId = pt.Id,
                    Month = pt.ProcedureDate.Month,
                    pt.HospitalId,
                    HospitalName = pt.Hospital != null ? pt.Hospital.FacilityName : null,
                    ProductCategoryId = l.Product != null ? l.Product.ProductCategoryId : null,
                    ProductCategoryName = l.Product != null && l.Product.ProductCategory != null
                        ? l.Product.ProductCategory.Name
                        : null,
                    pt.ImplantType,
                    l.Quantity,
                    l.LineTotal
                }))
                .ToListAsync();

            var report = new List<QuarterlyRollupReportDto>();

            foreach (var quarter in quarters)
            {
                var quarterMonths = months[quarter];

                var quarterPlans = plans.Where(p => quarterMonths.Contains(p.PeriodMonth)).ToList();
                var quarterActuals = actuals.Where(a => quarterMonths.Contains(a.Month)).ToList();

                // Every hospital + category that has either a target or revenue this quarter.
                var keys = quarterPlans
                    .Select(p => new { p.HospitalId, CategoryId = p.ProductCategoryId, p.HospitalName, p.ProductCategoryName })
                    .Concat(quarterActuals.Select(a => new
                    {
                        HospitalId = a.HospitalId ?? 0,
                        CategoryId = a.ProductCategoryId ?? 0,
                        a.HospitalName,
                        a.ProductCategoryName
                    }))
                    .GroupBy(k => new { k.HospitalId, k.CategoryId })
                    .Select(g => g.First());

                foreach (var key in keys)
                {
                    var plan = quarterPlans
                        .Where(p => p.HospitalId == key.HospitalId && p.ProductCategoryId == key.CategoryId)
                        .Sum(p => p.TargetAmount);

                    var rows = quarterActuals
                        .Where(a => (a.HospitalId ?? 0) == key.HospitalId && (a.ProductCategoryId ?? 0) == key.CategoryId)
                        .ToList();

                    var sold = rows.Sum(a => a.LineTotal);

                    report.Add(new QuarterlyRollupReportDto
                    {
                        Year = year,
                        Quarter = quarter,
                        QuarterName = "Q" + quarter,
                        HospitalId = key.HospitalId == 0 ? (int?)null : key.HospitalId,
                        HospitalName = key.HospitalName ?? "",
                        ProductCategoryId = key.CategoryId,
                        ProductCategoryName = string.IsNullOrWhiteSpace(key.ProductCategoryName)
                            ? "(Uncategorised)"
                            : key.ProductCategoryName,
                        TotalPlan = plan,
                        TotalSold = sold,
                        Variance = sold - plan,
                        PercentToPlan = plan > 0 ? (sold / plan) * 100 : 0,
                        // Distinct cases, not device lines.
                        TotalCases = rows.Select(a => a.CaseId).Distinct().Count(),
                        DeNovoCases = rows.Where(a => a.ImplantType == ImplantType.DeNovo)
                                          .Select(a => a.CaseId).Distinct().Count(),
                        GenChangeCases = rows.Where(a => a.ImplantType == ImplantType.GenChange)
                                             .Select(a => a.CaseId).Distinct().Count(),
                        TotalUnits = rows.Sum(a => a.Quantity),
                        HasPlan = plan > 0
                    });
                }
            }

            return report
                .OrderBy(r => r.Quarter)
                .ThenBy(r => r.HospitalName)
                .ThenBy(r => r.ProductCategoryName)
                .ToList();
        }
    }
}

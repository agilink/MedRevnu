using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using ATI.Revenue.Application.Dashboard.Dtos;
using ATI.Revenue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Dashboard
{
    /// <summary>
    /// Read model for the Revenue dashboard.
    /// </summary>
    /// <remarks>
    /// Revenue comes from ProcedureTransaction and targets come from ProductQuota. This
    /// previously read revenue from Case and targets from ProcedureQuota, while the
    /// reports read ProcedureTransaction and the only quota screen wrote ProductQuota -
    /// so the dashboard and the reports answered "how much did we bill this month" from
    /// different tables.
    ///
    /// A case is one ProcedureTransaction row with a line per device, which splits the two
    /// figures apart:
    ///  - case counts come from the case rows, since counting device lines would report a
    ///    three-device procedure as three cases;
    ///  - anything broken down by product or category comes from the lines, since one case
    ///    can span several categories.
    ///
    /// Device lines are always reached through the case, so ABP's soft-delete filter on
    /// the case applies; querying the line table directly would still return lines
    /// belonging to deleted cases.
    /// </remarks>
    public class RevenueDashboardAppService : ApplicationService, IRevenueDashboardAppService
    {
        private readonly IRepository<ProcedureTransaction, int> _procedureTransactionRepository;
        private readonly IRepository<ProductQuota, int> _productQuotaRepository;

        public RevenueDashboardAppService(
            IRepository<ProcedureTransaction, int> procedureTransactionRepository,
            IRepository<ProductQuota, int> productQuotaRepository)
        {
            _procedureTransactionRepository = procedureTransactionRepository;
            _productQuotaRepository = productQuotaRepository;
        }

        public async Task<DailyRevenueSummaryDto> GetDailyRevenueSummary(DateTime? date = null, int? hospitalId = null)
        {
            // Every filter is optional: the dashboard must render before anything is
            // chosen, so an absent date means today rather than a validation error.
            var day = (date ?? Abp.Timing.Clock.Now).Date;

            var cases = await QueryCases(hospitalId)
                .Where(pt => pt.ProcedureDate.Date == day)
                .Select(pt => new { pt.Id, pt.TotalAmount })
                .ToListAsync();

            var lines = await QueryLines(hospitalId)
                .Where(l => l.ProcedureDate.Date == day)
                .ToListAsync();

            return new DailyRevenueSummaryDto
            {
                Date = day,
                TotalRevenue = cases.Sum(c => c.TotalAmount),
                TotalCases = cases.Count,
                TransactionCount = cases.Count,
                RevenueByCategory = lines
                    .GroupBy(l => new { l.ProductCategoryId, l.ProductCategoryName })
                    .Select(g => new RevenueByCategoryDto
                    {
                        ProductCategoryId = g.Key.ProductCategoryId ?? 0,
                        ProductCategoryName = CategoryLabel(g.Key.ProductCategoryName),
                        Revenue = g.Sum(l => l.LineTotal),
                        // Distinct cases, so a case with two devices in one category counts once.
                        CaseCount = g.Select(l => l.CaseId).Distinct().Count(),
                        TransactionCount = g.Count()
                    })
                    .OrderByDescending(c => c.Revenue)
                    .ToList()
            };
        }

        public async Task<List<MonthlyRevenueByCategoryDto>> GetMonthlyRevenueByCategory(int? month = null, int? year = null, int? hospitalId = null)
        {
            var resolvedYear = year ?? Abp.Timing.Clock.Now.Year;
            var resolvedMonth = month ?? Abp.Timing.Clock.Now.Month;

            var lines = await QueryLines(hospitalId)
                .Where(l => l.ProcedureDate.Year == resolvedYear && l.ProcedureDate.Month == resolvedMonth)
                .ToListAsync();

            return lines
                .GroupBy(l => new { l.ProductCategoryId, l.ProductCategoryName, l.ImplantType })
                .Select(g => new MonthlyRevenueByCategoryDto
                {
                    ProductCategoryId = g.Key.ProductCategoryId ?? 0,
                    ProductCategoryName = CategoryLabel(g.Key.ProductCategoryName),
                    ImplantType = g.Key.ImplantType,
                    TotalRevenue = g.Sum(l => l.LineTotal),
                    CaseCount = g.Select(l => l.CaseId).Distinct().Count(),
                    TransactionCount = g.Count()
                })
                .OrderBy(r => r.ProductCategoryName)
                .ThenBy(r => r.ImplantType)
                .ToList();
        }

        public async Task<List<RevenueVsQuotaDto>> GetRevenueVsQuota(int? month = null, int? year = null, int? hospitalId = null)
        {
            var resolvedYear = year ?? Abp.Timing.Clock.Now.Year;
            var resolvedMonth = month ?? Abp.Timing.Clock.Now.Month;

            var quotas = await _productQuotaRepository.GetAll()
                .Where(q => q.PeriodYear == resolvedYear && q.PeriodMonth == resolvedMonth)
                .WhereIf(hospitalId.HasValue, q => q.HospitalId == hospitalId.Value)
                .Select(q => new
                {
                    q.HospitalId,
                    HospitalName = q.Hospital != null ? q.Hospital.FacilityName : null,
                    q.ProductCategoryId,
                    ProductCategoryName = q.ProductCategory != null ? q.ProductCategory.Name : null,
                    q.ProductId,
                    ProductName = q.Product != null ? q.Product.Name : null,
                    q.TargetAmount,
                    q.TargetUnits
                })
                .ToListAsync();

            // Quotas are per hospital and product category, so actuals must come from the
            // device lines - a case can contribute to more than one category.
            var actuals = await QueryLines(hospitalId)
                .Where(l => l.ProcedureDate.Year == resolvedYear && l.ProcedureDate.Month == resolvedMonth)
                .ToListAsync();

            var productLevelQuotaKeys = quotas
                .Where(q => q.ProductId.HasValue)
                .Select(q => (q.HospitalId, q.ProductCategoryId, ProductId: q.ProductId.Value))
                .ToHashSet();

            var result = new List<RevenueVsQuotaDto>();
            var matched = new HashSet<LineRow>();

            foreach (var quota in quotas)
            {
                var matching = actuals
                    .Where(a => a.HospitalId == quota.HospitalId
                                && (a.ProductCategoryId ?? 0) == quota.ProductCategoryId
                                && (quota.ProductId.HasValue
                                        ? a.ProductId == quota.ProductId.Value
                                        : !productLevelQuotaKeys.Contains((a.HospitalId, a.ProductCategoryId ?? 0, a.ProductId))))
                    .ToList();

                foreach (var row in matching)
                {
                    matched.Add(row);
                }

                result.Add(BuildRow(
                    quota.HospitalId, quota.HospitalName,
                    quota.ProductCategoryId, quota.ProductCategoryName,
                    quota.ProductId, quota.ProductName,
                    quota.TargetAmount, quota.TargetUnits,
                    matching, hasQuota: true));
            }

            // Revenue with no quota covering it, so it still shows on the dashboard.
            foreach (var group in actuals.Where(a => !matched.Contains(a))
                                         .GroupBy(a => new { a.HospitalId, a.HospitalName, a.ProductCategoryId, a.ProductCategoryName }))
            {
                result.Add(BuildRow(
                    group.Key.HospitalId, group.Key.HospitalName,
                    group.Key.ProductCategoryId ?? 0, group.Key.ProductCategoryName,
                    productId: null, productName: null,
                    targetAmount: 0m, targetUnits: null,
                    rows: group.ToList(), hasQuota: false));
            }

            return result
                .OrderBy(r => r.HospitalName)
                .ThenBy(r => r.ProductCategoryName)
                .ThenBy(r => r.ProductName)
                .ToList();
        }

        public async Task<List<RevenueTrendDto>> GetRevenueTrend(DateTime? startDate = null, DateTime? endDate = null, int? hospitalId = null)
        {
            // Absent bounds mean the current month to date.
            var to = (endDate ?? Abp.Timing.Clock.Now).Date;
            var from = (startDate ?? new DateTime(to.Year, to.Month, 1)).Date;

            var cases = await QueryCases(hospitalId)
                .Where(pt => pt.ProcedureDate >= from && pt.ProcedureDate < to.AddDays(1))
                .Select(pt => new { pt.ProcedureDate, pt.TotalAmount })
                .ToListAsync();

            return cases
                .GroupBy(c => c.ProcedureDate.Date)
                .Select(g => new RevenueTrendDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(c => c.TotalAmount),
                    CaseCount = g.Count(),
                    TransactionCount = g.Count()
                })
                .OrderBy(t => t.Date)
                .ToList();
        }

        /// <summary>
        /// The whole dashboard for one date range, in a single round trip.
        /// </summary>
        /// <remarks>
        /// Targets are monthly, so the planned figure sums the target of every month the
        /// range touches; a partial month still contributes its whole target, because a
        /// monthly target has no defensible per-day split. PlannedMonths reports which
        /// months were counted, so the page can say so rather than leave it to be guessed.
        /// </remarks>
        public async Task<RevenueDashboardDto> GetRevenueDashboard(RevenueDashboardInput input)
        {
            input = input ?? new RevenueDashboardInput();

            var today = Abp.Timing.Clock.Now.Date;
            var from = (input.FromDate ?? new DateTime(today.Year, today.Month, 1)).Date;
            var to = (input.ToDate ?? today).Date;

            if (to < from)
            {
                var swap = from;
                from = to;
                to = swap;
            }

            var upperBound = to.AddDays(1);

            var cases = await QueryCases(input.HospitalId)
                .Where(pt => pt.ProcedureDate >= from && pt.ProcedureDate < upperBound)
                .Select(pt => new
                {
                    pt.Id,
                    pt.ProcedureDate,
                    pt.HospitalId,
                    HospitalName = pt.Hospital != null ? pt.Hospital.FacilityName : null,
                    pt.PhysicianId,
                    PhysicianName = pt.Physician != null
                        ? ((pt.Physician.FIRST_NAME ?? "") + " " + (pt.Physician.LAST_NAME ?? "")).Trim()
                        : null,
                    PhysicianHospital = pt.Physician != null && pt.Physician.Facility != null
                        ? pt.Physician.Facility.FacilityName
                        : null,
                    pt.TotalAmount,
                    Units = pt.Products.Sum(l => (int?)l.Quantity) ?? 0
                })
                .ToListAsync();

            var lines = await QueryLines(input.HospitalId)
                .Where(l => l.ProcedureDate >= from && l.ProcedureDate < upperBound)
                .ToListAsync();

            // Every month the range touches, so a target counts once even when the range
            // starts or ends mid-month.
            var months = new List<DateTime>();
            for (var cursor = new DateTime(from.Year, from.Month, 1); cursor <= to; cursor = cursor.AddMonths(1))
            {
                months.Add(cursor);
            }

            var monthKeys = months.Select(m => m.Year * 100 + m.Month).ToList();

            var quotas = await _productQuotaRepository.GetAll()
                .WhereIf(input.HospitalId.HasValue, q => q.HospitalId == input.HospitalId.Value)
                .Where(q => monthKeys.Contains(q.PeriodYear * 100 + q.PeriodMonth))
                .Select(q => new
                {
                    q.ProductCategoryId,
                    ProductCategoryName = q.ProductCategory != null ? q.ProductCategory.Name : null,
                    q.TargetAmount
                })
                .ToListAsync();

            var totalSold = cases.Sum(c => c.TotalAmount);
            var totalPlanned = quotas.Sum(q => q.TargetAmount);

            var dto = new RevenueDashboardDto
            {
                FromDate = from,
                ToDate = to,
                TotalCases = cases.Count,
                TotalUnits = cases.Sum(c => c.Units),
                TotalSold = totalSold,
                TotalPlanned = totalPlanned,
                Variance = totalSold - totalPlanned,
                PercentAchieved = totalPlanned > 0 ? (totalSold / totalPlanned) * 100 : 0,
                HasPlan = totalPlanned > 0,
                PlannedMonths = months.Select(m => m.ToString("MMM yyyy")).ToList()
            };

            // Device type is the product category, which is the grain quotas are set at.
            // Sold comes from the device lines, because one case can span categories.
            var soldByCategory = lines
                .GroupBy(l => new { Id = l.ProductCategoryId ?? 0, Name = l.ProductCategoryName })
                .ToDictionary(g => g.Key.Id, g => new CategorySold
                {
                    Name = CategoryLabel(g.Key.Name),
                    Sold = g.Sum(l => l.LineTotal),
                    Cases = g.Select(l => l.CaseId).Distinct().Count(),
                    Units = g.Sum(l => l.Quantity)
                });

            var plannedByCategory = quotas
                .GroupBy(q => new { q.ProductCategoryId, q.ProductCategoryName })
                .ToDictionary(g => g.Key.ProductCategoryId, g => new CategoryPlanned
                {
                    Name = CategoryLabel(g.Key.ProductCategoryName),
                    Planned = g.Sum(q => q.TargetAmount)
                });

            var categoryIds = soldByCategory.Keys.Concat(plannedByCategory.Keys).Distinct().ToList();

            foreach (var categoryId in categoryIds)
            {
                soldByCategory.TryGetValue(categoryId, out var sold);
                plannedByCategory.TryGetValue(categoryId, out var planned);

                var soldAmount = sold != null ? sold.Sold : 0m;
                var plannedAmount = planned != null ? planned.Planned : 0m;

                dto.DeviceTypes.Add(new DeviceTypePerformanceDto
                {
                    ProductCategoryId = categoryId,
                    DeviceType = sold != null ? sold.Name : (planned != null ? planned.Name : "(Uncategorised)"),
                    Sold = soldAmount,
                    Planned = plannedAmount,
                    Variance = soldAmount - plannedAmount,
                    PercentAchieved = plannedAmount > 0 ? (soldAmount / plannedAmount) * 100 : 0,
                    Cases = sold != null ? sold.Cases : 0,
                    Units = sold != null ? sold.Units : 0,
                    HasPlan = plannedAmount > 0
                });
            }

            dto.DeviceTypes = dto.DeviceTypes
                .OrderByDescending(d => d.Sold)
                .ThenBy(d => d.DeviceType)
                .ToList();

            dto.TopPhysicians = cases
                .GroupBy(c => new { c.PhysicianId, c.PhysicianName, c.PhysicianHospital })
                .Select(g => new CollectionRowDto
                {
                    Id = g.Key.PhysicianId,
                    Name = string.IsNullOrWhiteSpace(g.Key.PhysicianName) ? "(Unnamed)" : g.Key.PhysicianName,
                    SecondaryName = g.Key.PhysicianHospital ?? "",
                    Cases = g.Count(),
                    Units = g.Sum(c => c.Units),
                    Collection = g.Sum(c => c.TotalAmount),
                    SharePercent = totalSold > 0 ? (g.Sum(c => c.TotalAmount) / totalSold) * 100 : 0
                })
                .OrderByDescending(r => r.Collection)
                .Take(10)
                .ToList();

            dto.TopHospitals = cases
                .GroupBy(c => new { c.HospitalId, c.HospitalName })
                .Select(g => new CollectionRowDto
                {
                    Id = g.Key.HospitalId ?? 0,
                    Name = string.IsNullOrWhiteSpace(g.Key.HospitalName) ? "(No hospital)" : g.Key.HospitalName,
                    SecondaryName = "",
                    Cases = g.Count(),
                    Units = g.Sum(c => c.Units),
                    Collection = g.Sum(c => c.TotalAmount),
                    SharePercent = totalSold > 0 ? (g.Sum(c => c.TotalAmount) / totalSold) * 100 : 0
                })
                .OrderByDescending(r => r.Collection)
                .Take(10)
                .ToList();

            var daily = cases
                .GroupBy(c => c.ProcedureDate.Date)
                .Select(g => new DailyRevenueRowDto
                {
                    Date = g.Key,
                    Cases = g.Count(),
                    Units = g.Sum(c => c.Units),
                    Revenue = g.Sum(c => c.TotalAmount)
                })
                .OrderBy(d => d.Date)
                .ToList();

            // Bars are relative to the busiest day, so a quiet day still reads as small
            // rather than empty.
            var busiestDay = daily.Count > 0 ? daily.Max(d => d.Revenue) : 0m;
            foreach (var day in daily)
            {
                day.SharePercent = busiestDay > 0 ? (day.Revenue / busiestDay) * 100 : 0;
            }

            dto.DailyRevenue = daily;

            return dto;
        }

        private class CategorySold
        {
            public string Name { get; set; }
            public decimal Sold { get; set; }
            public int Cases { get; set; }
            public int Units { get; set; }
        }

        private class CategoryPlanned
        {
            public string Name { get; set; }
            public decimal Planned { get; set; }
        }

        private IQueryable<ProcedureTransaction> QueryCases(int? hospitalId)
        {
            return _procedureTransactionRepository.GetAll()
                .WhereIf(hospitalId.HasValue, pt => pt.HospitalId == hospitalId.Value);
        }

        /// <summary>
        /// Device lines flattened out of their cases, carrying the case-level fields the
        /// breakdowns need.
        /// </summary>
        private IQueryable<LineRow> QueryLines(int? hospitalId)
        {
            return QueryCases(hospitalId)
                .SelectMany(pt => pt.Products.Select(l => new LineRow
                {
                    CaseId = pt.Id,
                    ProcedureDate = pt.ProcedureDate,
                    HospitalId = pt.HospitalId ?? 0,
                    HospitalName = pt.Hospital != null ? pt.Hospital.FacilityName : null,
                    ImplantType = pt.ImplantType,
                    ProductId = l.ProductId,
                    ProductName = l.Product != null ? l.Product.Name : null,
                    ProductCategoryId = l.Product != null ? l.Product.ProductCategoryId : null,
                    ProductCategoryName = l.Product != null && l.Product.ProductCategory != null
                        ? l.Product.ProductCategory.Name
                        : null,
                    Quantity = l.Quantity,
                    LineTotal = l.LineTotal
                }));
        }

        private static RevenueVsQuotaDto BuildRow(
            int hospitalId, string hospitalName,
            int productCategoryId, string productCategoryName,
            int? productId, string productName,
            decimal targetAmount, int? targetUnits,
            List<LineRow> rows, bool hasQuota)
        {
            var actualRevenue = rows.Sum(r => r.LineTotal);

            return new RevenueVsQuotaDto
            {
                HospitalId = hospitalId,
                HospitalName = hospitalName ?? "",
                ProductCategoryId = productCategoryId,
                ProductCategoryName = CategoryLabel(productCategoryName),
                ProductId = productId,
                ProductName = productName ?? "",
                TargetAmount = targetAmount,
                TargetUnits = targetUnits,
                ActualRevenue = actualRevenue,
                ActualUnits = rows.Sum(r => r.Quantity),
                // Distinct cases, not device lines.
                TransactionCount = rows.Select(r => r.CaseId).Distinct().Count(),
                Variance = actualRevenue - targetAmount,
                PercentageAchieved = targetAmount > 0 ? (actualRevenue / targetAmount) * 100 : 0,
                HasQuota = hasQuota
            };
        }

        private static string CategoryLabel(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? "(Uncategorised)" : name;
        }

        /// <summary>
        /// Reference-equality class so each device line can be tracked individually when
        /// working out which revenue a quota line has already claimed.
        /// </summary>
        private class LineRow
        {
            public int CaseId { get; set; }
            public DateTime ProcedureDate { get; set; }
            public int HospitalId { get; set; }
            public string HospitalName { get; set; }
            public Domain.Enums.ImplantType ImplantType { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int? ProductCategoryId { get; set; }
            public string ProductCategoryName { get; set; }
            public int Quantity { get; set; }
            public decimal LineTotal { get; set; }
        }
    }
}

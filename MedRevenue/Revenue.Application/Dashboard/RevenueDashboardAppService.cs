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
    /// Revenue comes from ProcedureTransaction and targets come from ProductQuota.
    /// This previously read revenue from Case and targets from ProcedureQuota, while
    /// the reports read ProcedureTransaction and the only quota screen in the menu
    /// wrote ProductQuota - so the dashboard and the reports answered "how much did
    /// we bill this month" from different tables, and quota-vs-actual compared
    /// against targets nobody could enter.
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

        public async Task<DailyRevenueSummaryDto> GetDailyRevenueSummary(DateTime date, int? hospitalId = null)
        {
            var rows = await QueryTransactions(hospitalId)
                .Where(pt => pt.ProcedureDate.Date == date.Date)
                .Select(pt => new
                {
                    ProductCategoryId = pt.Product.ProductCategoryId,
                    ProductCategoryName = pt.Product.ProductCategory != null ? pt.Product.ProductCategory.Name : null,
                    pt.Quantity,
                    pt.TotalAmount
                })
                .ToListAsync();

            return new DailyRevenueSummaryDto
            {
                Date = date.Date,
                TotalRevenue = rows.Sum(r => r.TotalAmount),
                TotalCases = rows.Sum(r => r.Quantity),
                TransactionCount = rows.Count,
                RevenueByCategory = rows
                    .GroupBy(r => new { r.ProductCategoryId, r.ProductCategoryName })
                    .Select(g => new RevenueByCategoryDto
                    {
                        ProductCategoryId = g.Key.ProductCategoryId ?? 0,
                        ProductCategoryName = CategoryLabel(g.Key.ProductCategoryName),
                        Revenue = g.Sum(r => r.TotalAmount),
                        CaseCount = g.Sum(r => r.Quantity),
                        TransactionCount = g.Count()
                    })
                    .OrderByDescending(c => c.Revenue)
                    .ToList()
            };
        }

        public async Task<List<MonthlyRevenueByCategoryDto>> GetMonthlyRevenueByCategory(int month, int year, int? hospitalId = null)
        {
            var rows = await QueryTransactions(hospitalId)
                .Where(pt => pt.ProcedureDate.Year == year && pt.ProcedureDate.Month == month)
                .Select(pt => new
                {
                    ProductCategoryId = pt.Product.ProductCategoryId,
                    ProductCategoryName = pt.Product.ProductCategory != null ? pt.Product.ProductCategory.Name : null,
                    pt.ImplantType,
                    pt.Quantity,
                    pt.TotalAmount
                })
                .ToListAsync();

            return rows
                .GroupBy(r => new { r.ProductCategoryId, r.ProductCategoryName, r.ImplantType })
                .Select(g => new MonthlyRevenueByCategoryDto
                {
                    ProductCategoryId = g.Key.ProductCategoryId ?? 0,
                    ProductCategoryName = CategoryLabel(g.Key.ProductCategoryName),
                    ImplantType = g.Key.ImplantType,
                    TotalRevenue = g.Sum(r => r.TotalAmount),
                    CaseCount = g.Sum(r => r.Quantity),
                    TransactionCount = g.Count()
                })
                .OrderBy(r => r.ProductCategoryName)
                .ThenBy(r => r.ImplantType)
                .ToList();
        }

        public async Task<List<RevenueVsQuotaDto>> GetRevenueVsQuota(int month, int year, int? hospitalId = null)
        {
            var quotas = await _productQuotaRepository.GetAll()
                .Where(q => q.PeriodYear == year && q.PeriodMonth == month)
                .WhereIf(hospitalId.HasValue, q => q.HospitalId == hospitalId.Value)
                .Select(q => new
                {
                    q.Id,
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

            var actuals = await QueryTransactions(hospitalId)
                .Where(pt => pt.ProcedureDate.Year == year && pt.ProcedureDate.Month == month)
                .Select(pt => new ActualRow
                {
                    HospitalId = pt.HospitalId ?? 0,
                    HospitalName = pt.Hospital != null ? pt.Hospital.FacilityName : null,
                    ProductCategoryId = pt.Product.ProductCategoryId ?? 0,
                    ProductCategoryName = pt.Product.ProductCategory != null ? pt.Product.ProductCategory.Name : null,
                    ProductId = pt.ProductId,
                    ProductName = pt.Product.Name,
                    Quantity = pt.Quantity,
                    TotalAmount = pt.TotalAmount
                })
                .ToListAsync();

            // Products that have their own quota line are excluded from the
            // category-level line, so revenue is never counted against two targets.
            var productLevelQuotaKeys = quotas
                .Where(q => q.ProductId.HasValue)
                .Select(q => (q.HospitalId, q.ProductCategoryId, ProductId: q.ProductId.Value))
                .ToHashSet();

            var result = new List<RevenueVsQuotaDto>();
            var matchedActuals = new HashSet<ActualRow>();

            foreach (var quota in quotas)
            {
                var matching = actuals
                    .Where(a => a.HospitalId == quota.HospitalId
                                && a.ProductCategoryId == quota.ProductCategoryId
                                && (quota.ProductId.HasValue
                                        ? a.ProductId == quota.ProductId.Value
                                        : !productLevelQuotaKeys.Contains((a.HospitalId, a.ProductCategoryId, a.ProductId))))
                    .ToList();

                foreach (var row in matching)
                {
                    matchedActuals.Add(row);
                }

                result.Add(BuildRow(
                    quota.HospitalId,
                    quota.HospitalName,
                    quota.ProductCategoryId,
                    quota.ProductCategoryName,
                    quota.ProductId,
                    quota.ProductName,
                    quota.TargetAmount,
                    quota.TargetUnits,
                    matching,
                    hasQuota: true));
            }

            // Revenue with no quota covering it, so it still shows on the dashboard.
            var unquoted = actuals
                .Where(a => !matchedActuals.Contains(a))
                .GroupBy(a => new { a.HospitalId, a.HospitalName, a.ProductCategoryId, a.ProductCategoryName });

            foreach (var group in unquoted)
            {
                result.Add(BuildRow(
                    group.Key.HospitalId,
                    group.Key.HospitalName,
                    group.Key.ProductCategoryId,
                    group.Key.ProductCategoryName,
                    productId: null,
                    productName: null,
                    targetAmount: 0m,
                    targetUnits: null,
                    rows: group.ToList(),
                    hasQuota: false));
            }

            return result
                .OrderBy(r => r.HospitalName)
                .ThenBy(r => r.ProductCategoryName)
                .ThenBy(r => r.ProductName)
                .ToList();
        }

        public async Task<List<RevenueTrendDto>> GetRevenueTrend(DateTime startDate, DateTime endDate, int? hospitalId = null)
        {
            var rows = await QueryTransactions(hospitalId)
                .Where(pt => pt.ProcedureDate >= startDate.Date && pt.ProcedureDate < endDate.Date.AddDays(1))
                .Select(pt => new { pt.ProcedureDate, pt.Quantity, pt.TotalAmount })
                .ToListAsync();

            return rows
                .GroupBy(r => r.ProcedureDate.Date)
                .Select(g => new RevenueTrendDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(r => r.TotalAmount),
                    CaseCount = g.Sum(r => r.Quantity),
                    TransactionCount = g.Count()
                })
                .OrderBy(t => t.Date)
                .ToList();
        }

        private IQueryable<ProcedureTransaction> QueryTransactions(int? hospitalId)
        {
            return _procedureTransactionRepository.GetAll()
                .WhereIf(hospitalId.HasValue, pt => pt.HospitalId == hospitalId.Value);
        }

        private static RevenueVsQuotaDto BuildRow(
            int hospitalId,
            string hospitalName,
            int productCategoryId,
            string productCategoryName,
            int? productId,
            string productName,
            decimal targetAmount,
            int? targetUnits,
            List<ActualRow> rows,
            bool hasQuota)
        {
            var actualRevenue = rows.Sum(r => r.TotalAmount);

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
                TransactionCount = rows.Count,
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
        /// Reference-equality class so each transaction row can be tracked individually
        /// when working out which revenue a quota line has already claimed.
        /// </summary>
        private class ActualRow
        {
            public int HospitalId { get; set; }
            public string HospitalName { get; set; }
            public int ProductCategoryId { get; set; }
            public string ProductCategoryName { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal TotalAmount { get; set; }
        }
    }
}

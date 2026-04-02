using Abp.Application.Services;
using Abp.Domain.Repositories;
using ATI.Revenue.Application.Dashboard.Dtos;
using ATI.Revenue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Dashboard
{
    public class RevenueDashboardAppService : ApplicationService, IRevenueDashboardAppService
    {
        private readonly IRepository<Case, int> _caseRepository;
        private readonly IRepository<ProcedureType, int> _procedureTypeRepository;
        private readonly IRepository<ProcedureQuota, int> _procedureQuotaRepository;

        public RevenueDashboardAppService(
            IRepository<Case, int> caseRepository,
            IRepository<ProcedureType, int> procedureTypeRepository,
            IRepository<ProcedureQuota, int> procedureQuotaRepository)
        {
            _caseRepository = caseRepository;
            _procedureTypeRepository = procedureTypeRepository;
            _procedureQuotaRepository = procedureQuotaRepository;
        }

        public async Task<DailyRevenueSummaryDto> GetDailyRevenueSummary(DateTime date, int? facilityId = null)
        {
            var query = _caseRepository.GetAll()
                .Include(c => c.ProcedureType)
                .Where(c => c.ProcedureDate.HasValue && c.ProcedureDate.Value.Date == date.Date);

            if (facilityId.HasValue)
            {
                query = query.Where(c => c.FacilityId == facilityId.Value);
            }

            var cases = await query.ToListAsync();

            var summary = new DailyRevenueSummaryDto
            {
                Date = date,
                TotalRevenue = cases.Sum(c => c.TotalAmount),
                TotalCases = cases.Count,
                RevenueByCategory = cases
                    .Where(c => c.ProcedureType != null)
                    .GroupBy(c => c.ProcedureType.CategoryGroup)
                    .Select(g => new RevenueByCategoryDto
                    {
                        CategoryGroup = g.Key,
                        CategoryName = g.Key.ToString(),
                        Revenue = g.Sum(c => c.TotalAmount),
                        CaseCount = g.Count()
                    })
                    .OrderBy(r => r.CategoryGroup)
                    .ToList()
            };

            return summary;
        }

        public async Task<List<RevenueByProcedureTypeDto>> GetMonthlyRevenueByProcedureType(int month, int year, int? facilityId = null)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var query = _caseRepository.GetAll()
                .Include(c => c.ProcedureType)
                .Where(c => c.ProcedureDate.HasValue &&
                           c.ProcedureDate.Value >= startDate &&
                           c.ProcedureDate.Value <= endDate &&
                           c.ProcedureTypeId.HasValue);

            if (facilityId.HasValue)
            {
                query = query.Where(c => c.FacilityId == facilityId.Value);
            }

            var cases = await query.ToListAsync();

            var revenueByProcedureType = cases
                .GroupBy(c => new
                {
                    c.ProcedureTypeId,
                    c.ProcedureType.Name,
                    c.ProcedureType.Code,
                    CategoryGroup = c.ProcedureType.CategoryGroup
                })
                .Select(g => new RevenueByProcedureTypeDto
                {
                    ProcedureTypeId = g.Key.ProcedureTypeId.Value,
                    ProcedureTypeName = g.Key.Name,
                    ProcedureTypeCode = g.Key.Code,
                    CategoryGroupName = g.Key.CategoryGroup.ToString(),
                    TotalRevenue = g.Sum(c => c.TotalAmount),
                    CaseCount = g.Count()
                })
                .OrderByDescending(r => r.TotalRevenue)
                .ToList();

            return revenueByProcedureType;
        }

        public async Task<List<RevenueVsQuotaDto>> GetRevenueVsQuota(int month, int year, int? facilityId = null)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Get all active quotas for the period
            var quotasQuery = _procedureQuotaRepository.GetAll()
                .Include(q => q.ProcedureType)
                .Where(q => q.StartDate <= endDate && q.EndDate >= startDate);

            if (facilityId.HasValue)
            {
                quotasQuery = quotasQuery.Where(q => q.FacilityId == facilityId.Value || q.FacilityId == null);
            }
            else
            {
                quotasQuery = quotasQuery.Where(q => q.FacilityId == null);
            }

            var quotas = await quotasQuery.ToListAsync();

            // Get actual revenue for the period
            var revenueQuery = _caseRepository.GetAll()
                .Include(c => c.ProcedureType)
                .Where(c => c.ProcedureDate.HasValue &&
                           c.ProcedureDate.Value >= startDate &&
                           c.ProcedureDate.Value <= endDate &&
                           c.ProcedureTypeId.HasValue);

            if (facilityId.HasValue)
            {
                revenueQuery = revenueQuery.Where(c => c.FacilityId == facilityId.Value);
            }

            var cases = await revenueQuery.ToListAsync();

            var revenueByProcedure = cases
                .GroupBy(c => c.ProcedureTypeId.Value)
                .ToDictionary(g => g.Key, g => new
                {
                    Revenue = g.Sum(c => c.TotalAmount),
                    Count = g.Count()
                });

            var result = quotas.Select(q =>
            {
                var actualRevenue = revenueByProcedure.ContainsKey(q.ProcedureTypeId)
                    ? revenueByProcedure[q.ProcedureTypeId].Revenue
                    : 0;

                var caseCount = revenueByProcedure.ContainsKey(q.ProcedureTypeId)
                    ? revenueByProcedure[q.ProcedureTypeId].Count
                    : 0;

                return new RevenueVsQuotaDto
                {
                    ProcedureTypeId = q.ProcedureTypeId,
                    ProcedureTypeName = q.ProcedureType.Name,
                    ProcedureTypeCode = q.ProcedureType.Code,
                    CategoryGroupName = q.ProcedureType.CategoryGroup.ToString(),
                    QuotaValue = q.QuotaValue,
                    ActualRevenue = actualRevenue,
                    Variance = actualRevenue - q.QuotaValue,
                    PercentageAchieved = q.QuotaValue > 0 ? (actualRevenue / q.QuotaValue) * 100 : 0,
                    CaseCount = caseCount
                };
            })
            .OrderBy(r => r.CategoryGroupName)
            .ThenBy(r => r.ProcedureTypeName)
            .ToList();

            return result;
        }

        public async Task<List<RevenueTrendDto>> GetRevenueTrend(DateTime startDate, DateTime endDate, int? facilityId = null)
        {
            var query = _caseRepository.GetAll()
                .Where(c => c.ProcedureDate.HasValue &&
                           c.ProcedureDate.Value >= startDate &&
                           c.ProcedureDate.Value <= endDate);

            if (facilityId.HasValue)
            {
                query = query.Where(c => c.FacilityId == facilityId.Value);
            }

            var cases = await query.ToListAsync();

            var trend = cases
                .GroupBy(c => c.ProcedureDate.Value.Date)
                .Select(g => new RevenueTrendDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(c => c.TotalAmount),
                    CaseCount = g.Count()
                })
                .OrderBy(t => t.Date)
                .ToList();

            return trend;
        }
    }
}

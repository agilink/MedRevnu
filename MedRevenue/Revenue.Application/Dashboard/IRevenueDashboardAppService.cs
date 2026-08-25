using Abp.Application.Services;
using ATI.Revenue.Application.Dashboard.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Dashboard
{
    public interface IRevenueDashboardAppService : IApplicationService
    {
        Task<DailyRevenueSummaryDto> GetDailyRevenueSummary(DateTime? date = null, int? hospitalId = null);
        Task<List<MonthlyRevenueByCategoryDto>> GetMonthlyRevenueByCategory(int? month = null, int? year = null, int? hospitalId = null);
        Task<List<RevenueVsQuotaDto>> GetRevenueVsQuota(int? month = null, int? year = null, int? hospitalId = null);
        Task<List<RevenueTrendDto>> GetRevenueTrend(DateTime? startDate = null, DateTime? endDate = null, int? hospitalId = null);
    }
}

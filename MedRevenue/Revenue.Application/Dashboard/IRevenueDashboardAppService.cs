using Abp.Application.Services;
using ATI.Revenue.Application.Dashboard.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Dashboard
{
    public interface IRevenueDashboardAppService : IApplicationService
    {
        Task<DailyRevenueSummaryDto> GetDailyRevenueSummary(DateTime date, int? facilityId = null);
        Task<List<RevenueByProcedureTypeDto>> GetMonthlyRevenueByProcedureType(int month, int year, int? facilityId = null);
        Task<List<RevenueVsQuotaDto>> GetRevenueVsQuota(int month, int year, int? facilityId = null);
        Task<List<RevenueTrendDto>> GetRevenueTrend(DateTime startDate, DateTime endDate, int? facilityId = null);
    }
}

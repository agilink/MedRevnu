using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ATI.MultiTenancy.HostDashboard.Dto;

namespace ATI.MultiTenancy.HostDashboard
{
    public interface IIncomeStatisticsService
    {
        Task<List<IncomeStastistic>> GetIncomeStatisticsData(DateTime startDate, DateTime endDate,
            ChartDateInterval dateInterval);
    }
}
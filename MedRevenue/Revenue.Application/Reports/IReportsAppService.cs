using Abp.Application.Services;
using ATI.Revenue.Application.Reports.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Reports
{
    public interface IReportsAppService : IApplicationService
    {
        Task<List<RateChartReportDto>> GetRateChartReport(RateChartReportInput input);
        Task<List<MonthlyRevenueReportDto>> GetMonthlyRevenueReport(MonthlyRevenueReportInput input);
        Task<List<CasesByPersonReportDto>> GetCasesByPersonReport(CasesByPersonReportInput input);
        Task<List<TransactionAmountReportDto>> GetTransactionAmountReport(TransactionAmountReportInput input);
        Task<List<QuarterlyRollupReportDto>> GetQuarterlyRollupReport(QuarterlyRollupReportInput input);
    }
}

using ATI.Dto;
using ATI.Revenue.Application.Reports.Dtos;
using System.Collections.Generic;

namespace ATI.Revenue.Application.Reports.Exporting
{
    public interface IRevenueReportsExcelExporter
    {
        FileDto ExportRateChart(List<RateChartReportDto> rows);
        FileDto ExportMonthlyRevenue(List<MonthlyRevenueReportDto> rows);
        FileDto ExportCasesByPerson(List<CasesByPersonReportDto> rows);
        FileDto ExportTransactionAmount(List<TransactionAmountReportDto> rows);
        FileDto ExportQuarterlyRollup(List<QuarterlyRollupReportDto> rows);
    }
}

using ATI.DataExporting.Excel.MiniExcel;
using ATI.Dto;
using ATI.Revenue.Application.Reports.Dtos;
using ATI.Revenue.Domain.Enums;
using ATI.Storage;
using System.Collections.Generic;

namespace ATI.Revenue.Application.Reports.Exporting
{
    /// <summary>
    /// Excel export for the Revenue reports.
    /// </summary>
    /// <remarks>
    /// The client currently keeps this data in a workbook, so being able to take a report
    /// back out to Excel is what lets them stop maintaining it by hand while still
    /// sharing numbers the way they already do.
    ///
    /// Uses the application's existing MiniExcel exporter base, so the files download
    /// through the same temp-file mechanism as every other export in the product.
    /// </remarks>
    public class RevenueReportsExcelExporter : MiniExcelExcelExporterBase, IRevenueReportsExcelExporter
    {
        public RevenueReportsExcelExporter(ITempFileCacheManager tempFileCacheManager)
            : base(tempFileCacheManager)
        {
        }

        public FileDto ExportRateChart(List<RateChartReportDto> rows)
        {
            var items = new List<Dictionary<string, object>>();

            foreach (var row in rows)
            {
                items.Add(new Dictionary<string, object>
                {
                    { "Product Category", row.ProductCategoryName },
                    { "Product Code", row.ProductCode },
                    { "Product", row.ProductName },
                    { "Base Price", row.BasePrice },
                    { "System", row.IsSystem ? "Yes" : "No" }
                });
            }

            return CreateExcelPackage("RateChart.xlsx", items);
        }

        public FileDto ExportMonthlyRevenue(List<MonthlyRevenueReportDto> rows)
        {
            var items = new List<Dictionary<string, object>>();

            foreach (var row in rows)
            {
                items.Add(new Dictionary<string, object>
                {
                    { "Date", row.ProcedureDate.ToString("yyyy-MM-dd") },
                    { "Product Category", row.ProductCategoryName },
                    { "Daily Revenue", row.DailyRevenue },
                    { "Transactions", row.TransactionCount }
                });
            }

            return CreateExcelPackage("MonthlyRevenue.xlsx", items);
        }

        public FileDto ExportCasesByPerson(List<CasesByPersonReportDto> rows)
        {
            var items = new List<Dictionary<string, object>>();

            foreach (var row in rows)
            {
                items.Add(new Dictionary<string, object>
                {
                    { "Physician", row.PhysicianName },
                    { "Hospital", row.HospitalName },
                    { "Total Cases", row.TotalCases },
                    { "De Novo", row.DeNovoCases },
                    { "Gen Changes", row.GenChangeCases },
                    { "Total Revenue", row.TotalRevenue }
                });
            }

            return CreateExcelPackage("CasesByPerson.xlsx", items);
        }

        public FileDto ExportTransactionAmount(List<TransactionAmountReportDto> rows)
        {
            var items = new List<Dictionary<string, object>>();

            foreach (var row in rows)
            {
                items.Add(new Dictionary<string, object>
                {
                    { "Physician", row.PhysicianName },
                    { "Product Category", row.ProductCategoryName },
                    { "Implant Type", Describe(row.ImplantType) },
                    { "Total Cases", row.TotalCases },
                    { "Total Amount", row.TotalAmount },
                    { "Average Amount", row.AverageAmount }
                });
            }

            return CreateExcelPackage("TransactionAmount.xlsx", items);
        }

        public FileDto ExportQuarterlyRollup(List<QuarterlyRollupReportDto> rows)
        {
            var items = new List<Dictionary<string, object>>();

            foreach (var row in rows)
            {
                items.Add(new Dictionary<string, object>
                {
                    { "Year", row.Year },
                    { "Quarter", row.QuarterName },
                    { "Hospital", row.HospitalName },
                    { "Product Category", row.ProductCategoryName },
                    // Blank rather than a misleading 0.00 where no target was set.
                    { "Total Plan", row.HasPlan ? (object)row.TotalPlan : "" },
                    { "Total Sold", row.TotalSold },
                    { "Variance", row.Variance },
                    { "% to Plan", row.HasPlan ? (object)row.PercentToPlan : "" },
                    { "Total Cases", row.TotalCases },
                    { "De Novo", row.DeNovoCases },
                    { "Gen Changes", row.GenChangeCases }
                });
            }

            return CreateExcelPackage("QuarterlyRollup.xlsx", items);
        }

        private static string Describe(ImplantType implantType)
        {
            return implantType == ImplantType.GenChange ? "Gen Change" : "De Novo";
        }
    }
}

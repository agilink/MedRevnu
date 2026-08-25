using System;

namespace ATI.Revenue.Application.Reports.Dtos
{
    /// <summary>
    /// Monthly Revenue: Daily revenue by ProductCategory for each month
    /// </summary>
    public class MonthlyRevenueReportDto
    {
        public DateTime ProcedureDate { get; set; }
        public string ProductCategoryName { get; set; }
        public decimal DailyRevenue { get; set; }
        public int TransactionCount { get; set; }
    }

    public class MonthlyRevenueReportInput
    {
        /// <summary>Optional. Defaults to the current year.</summary>
        public int? Year { get; set; }

        /// <summary>Optional. Defaults to the current month.</summary>
        public int? Month { get; set; }
        public int? HospitalId { get; set; }
    }
}

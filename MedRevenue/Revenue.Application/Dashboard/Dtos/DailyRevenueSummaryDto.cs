using System;
using System.Collections.Generic;

namespace ATI.Revenue.Application.Dashboard.Dtos
{
    public class DailyRevenueSummaryDto
    {
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }

        /// <summary>Number of cases, i.e. the sum of transaction quantities.</summary>
        public int TotalCases { get; set; }

        public int TransactionCount { get; set; }
        public List<RevenueByCategoryDto> RevenueByCategory { get; set; }

        public DailyRevenueSummaryDto()
        {
            RevenueByCategory = new List<RevenueByCategoryDto>();
        }
    }

    public class RevenueByCategoryDto
    {
        public int ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public decimal Revenue { get; set; }
        public int CaseCount { get; set; }
        public int TransactionCount { get; set; }
    }
}

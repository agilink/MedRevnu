using ATI.Revenue.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ATI.Revenue.Application.Dashboard.Dtos
{
    public class DailyRevenueSummaryDto
    {
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCases { get; set; }
        public List<RevenueByCategoryDto> RevenueByCategory { get; set; }

        public DailyRevenueSummaryDto()
        {
            RevenueByCategory = new List<RevenueByCategoryDto>();
        }
    }

    public class RevenueByCategoryDto
    {
        public CategoryGroup CategoryGroup { get; set; }
        public string CategoryName { get; set; }
        public decimal Revenue { get; set; }
        public int CaseCount { get; set; }
    }
}

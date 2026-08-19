using System;

namespace ATI.Revenue.Application.Dashboard.Dtos
{
    public class RevenueTrendDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }

        /// <summary>Number of cases, i.e. the sum of transaction quantities.</summary>
        public int CaseCount { get; set; }

        public int TransactionCount { get; set; }
    }
}

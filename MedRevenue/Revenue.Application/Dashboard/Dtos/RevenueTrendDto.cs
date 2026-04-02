using System;

namespace ATI.Revenue.Application.Dashboard.Dtos
{
    public class RevenueTrendDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int CaseCount { get; set; }
    }
}

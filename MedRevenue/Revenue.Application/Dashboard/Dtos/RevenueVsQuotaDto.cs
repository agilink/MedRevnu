namespace ATI.Revenue.Application.Dashboard.Dtos
{
    public class RevenueVsQuotaDto
    {
        public int ProcedureTypeId { get; set; }
        public string ProcedureTypeName { get; set; }
        public string ProcedureTypeCode { get; set; }
        public string CategoryGroupName { get; set; }
        public decimal QuotaValue { get; set; }
        public decimal ActualRevenue { get; set; }
        public decimal Variance { get; set; }
        public decimal PercentageAchieved { get; set; }
        public int CaseCount { get; set; }
    }
}

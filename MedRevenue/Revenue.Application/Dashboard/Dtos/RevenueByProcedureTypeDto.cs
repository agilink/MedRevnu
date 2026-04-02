namespace ATI.Revenue.Application.Dashboard.Dtos
{
    public class RevenueByProcedureTypeDto
    {
        public int ProcedureTypeId { get; set; }
        public string ProcedureTypeName { get; set; }
        public string ProcedureTypeCode { get; set; }
        public string CategoryGroupName { get; set; }
        public decimal TotalRevenue { get; set; }
        public int CaseCount { get; set; }
    }
}

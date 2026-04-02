namespace ATI.Revenue.Application.Reports.Dtos
{
    /// <summary>
    /// Transaction Amount: By physician per product type for last year
    /// </summary>
    public class TransactionAmountReportDto
    {
        public string PhysicianName { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProcedureType { get; set; }
        public int TotalCases { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AverageAmount { get; set; }
    }

    public class TransactionAmountReportInput
    {
        public int Year { get; set; }
        public int? PhysicianId { get; set; }
        public int? ProductCategoryId { get; set; }
    }
}

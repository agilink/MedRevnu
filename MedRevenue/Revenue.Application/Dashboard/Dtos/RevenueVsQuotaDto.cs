namespace ATI.Revenue.Application.Dashboard.Dtos
{
    /// <summary>
    /// A single quota line for a period: the target from ProductQuota against the
    /// revenue actually recorded in ProcedureTransaction.
    /// </summary>
    public class RevenueVsQuotaDto
    {
        public int HospitalId { get; set; }
        public string HospitalName { get; set; }

        public int ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }

        /// <summary>Set when the quota targets one specific device rather than the whole category.</summary>
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        public decimal TargetAmount { get; set; }
        public int? TargetUnits { get; set; }

        public decimal ActualRevenue { get; set; }
        public int ActualUnits { get; set; }
        public int TransactionCount { get; set; }

        public decimal Variance { get; set; }
        public decimal PercentageAchieved { get; set; }

        /// <summary>
        /// False for revenue that has no quota covering it. Those rows are still
        /// listed so revenue cannot go missing from the dashboard just because
        /// nobody set a target for it.
        /// </summary>
        public bool HasQuota { get; set; }
    }
}

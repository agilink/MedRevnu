namespace ATI.Revenue.Application.Reports.Dtos
{
    /// <summary>
    /// Quarterly rollup: three months of ProductQuota targets against the revenue
    /// actually recorded, per product category. This is the workbook's "Quarters" tab -
    /// Total Sold, Total Plan and Percent to Plan - which the client currently sums by
    /// hand.
    /// </summary>
    public class QuarterlyRollupReportDto
    {
        public int Year { get; set; }

        /// <summary>1 to 4.</summary>
        public int Quarter { get; set; }

        public string QuarterName { get; set; }

        public int ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }

        public int? HospitalId { get; set; }
        public string HospitalName { get; set; }

        /// <summary>Sum of the three monthly targets. Zero where no target was set.</summary>
        public decimal TotalPlan { get; set; }

        public decimal TotalSold { get; set; }
        public decimal Variance { get; set; }
        public decimal PercentToPlan { get; set; }

        public int TotalCases { get; set; }
        public int DeNovoCases { get; set; }
        public int GenChangeCases { get; set; }

        /// <summary>
        /// False where revenue was recorded with no quota covering it, so the row is
        /// still listed rather than dropped from the rollup.
        /// </summary>
        public bool HasPlan { get; set; }
    }

    public class QuarterlyRollupReportInput
    {
        /// <summary>Optional. Defaults to the current year.</summary>
        public int? Year { get; set; }

        /// <summary>Null returns all four quarters.</summary>
        public int? Quarter { get; set; }

        public int? HospitalId { get; set; }
    }
}

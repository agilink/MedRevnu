namespace ATI.Revenue.Application.Reports.Dtos
{
    /// <summary>
    /// Rate Chart: Products by ProductCategory with prices for a hospital
    /// </summary>
    public class RateChartReportDto
    {
        public string ProductCategoryName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal BasePrice { get; set; }

        /// <summary>
        /// The hospital's contracted price for this product, where one exists. Null when
        /// no hospital was chosen, or when this hospital has no contracted price.
        /// </summary>
        public decimal? ContractedPrice { get; set; }

        /// <summary>What a case at this hospital would actually be priced at.</summary>
        public decimal EffectivePrice { get; set; }

        /// <summary>Units of this device recorded so far this year.</summary>
        public int UnitsSoldThisYear { get; set; }

        /// <summary>Cases this device appeared on this year.</summary>
        public int CasesThisYear { get; set; }

        public bool IsSystem { get; set; }
    }

    public class RateChartReportInput
    {
        /// <summary>Optional. Null shows every hospital's catalogue.</summary>
        public int? HospitalId { get; set; }
    }
}

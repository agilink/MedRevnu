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
        public bool IsSystem { get; set; }
    }

    public class RateChartReportInput
    {
        public int HospitalId { get; set; }
    }
}

using ATI.Revenue.Domain.Enums;

namespace ATI.Revenue.Application.Dashboard.Dtos
{
    /// <summary>
    /// Monthly revenue split by product category and implant type, matching the
    /// De Novo / Gen Change breakdown the business reports on.
    /// </summary>
    public class MonthlyRevenueByCategoryDto
    {
        public int ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public ImplantType ImplantType { get; set; }
        public decimal TotalRevenue { get; set; }
        public int CaseCount { get; set; }
        public int TransactionCount { get; set; }
    }
}

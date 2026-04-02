using Abp.Application.Services.Dto;

namespace ATI.Revenue.Application.ProductQuotas.Dtos
{
    public class GetAllProductQuotasInput : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
        public int? YearFilter { get; set; }
        public int? MonthFilter { get; set; }
        public int? HospitalIdFilter { get; set; }
        public int? ProductCategoryIdFilter { get; set; }
    }
}

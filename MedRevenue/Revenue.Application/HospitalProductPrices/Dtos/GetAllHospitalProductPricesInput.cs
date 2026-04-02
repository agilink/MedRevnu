using Abp.Application.Services.Dto;

namespace ATI.Revenue.Application.HospitalProductPrices.Dtos
{
    public class GetAllHospitalProductPricesInput : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
        public int? HospitalIdFilter { get; set; }
        public int? ProductIdFilter { get; set; }
        public bool? IsActiveFilter { get; set; }
    }
}

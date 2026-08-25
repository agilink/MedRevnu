using Abp.Application.Services.Dto;

namespace ATI.Revenue.Application.Hospitals.Dtos
{
    public class GetAllHospitalsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}

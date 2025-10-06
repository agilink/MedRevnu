using Abp.Application.Services.Dto;

namespace ATI.Pharmacy.Dtos;

public class GetAllForPrescriptionLookupTableInput : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
}
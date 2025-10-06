using Abp.Application.Services.Dto;

namespace ATI.Pharmacy.Dtos;

public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
}
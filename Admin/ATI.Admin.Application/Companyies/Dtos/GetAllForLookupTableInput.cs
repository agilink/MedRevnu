using Abp.Application.Services.Dto;

namespace ATI.Admin.Application.Companies.Dtos;

public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
}
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace ATI.Pharmacy.Dtos;
public class GetUsersInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public int? Company { get; set; }
    public int? Facility { get; set; }

    public int? Role { get; set; }
}

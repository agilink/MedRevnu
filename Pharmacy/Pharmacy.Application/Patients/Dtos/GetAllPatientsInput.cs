using Abp.Application.Services.Dto;
using System;

namespace ATI.Pharmacy.Dtos;

public class GetAllPatientsInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public int? DefaultFacilityId { get; set; }

}
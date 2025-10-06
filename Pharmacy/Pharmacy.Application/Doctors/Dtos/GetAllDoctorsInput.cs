using Abp.Application.Services.Dto;
using System;

namespace ATI.Pharmacy.Dtos;

public class GetAllDoctorsInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }

    public int? MaxDoctorIDFilter { get; set; }
    public int? MinDoctorIDFilter { get; set; }
    public int? DefaultFacilityId { get; set; }
    public int? CompanyId { get; set; }

    public string? SpecialtyFilter { get; set; }

    public string? LicenseNumberFilter { get; set; }

}
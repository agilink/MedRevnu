using Abp.Application.Services.Dto;
using System;

namespace ATI.Pharmacy.Dtos;

public class GetAllPrescriptionsInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public int? PrescriptionStatus { get; set; }
    public int? defaultFacility { get; set; }

}
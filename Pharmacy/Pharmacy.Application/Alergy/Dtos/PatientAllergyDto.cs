

using Abp.Domain.Entities;
using ATI.Pharmacy.Domain.Entities;
using ATI.Pharmacy.Dtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATI.Pharmacy.Application.Alergy.Dtos;

public class PatientAllergyDto : Abp.Application.Services.Dto.EntityDto
{
    public int? TenantId { get; set; }
    public int PatientId { get; set; }
    public int? AllergyId { get; set; }
    public PatientDto Patient { get; set; }
    public AllergyDto? Allergy { get; set; }

    public string? Other { get; set; }
}
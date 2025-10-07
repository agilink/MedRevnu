using System;
using Abp.Application.Services.Dto;
using ATI.Pharmacy.Application.Patients;

namespace ATI.Pharmacy.Dtos;

public class MedicationDto : EntityDto
{
    public string Name { get; set; }
    public string MedicineId { get; set; }
    public string Description { get; set; }
    public string Instructions { get; set; }
    public decimal? Dosage { get; set; }

    public int? Quantity { get; set; }

    public int? RefillsAllowed { get; set; }
    public int? DosageRouteId { get; set; }
    public int MedicationCategoryId { get; set; }
    public bool IsDeleted { get; set; }
}
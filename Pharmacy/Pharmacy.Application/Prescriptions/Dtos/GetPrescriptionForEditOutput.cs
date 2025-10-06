using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace ATI.Pharmacy.Dtos;

public class GetPrescriptionForEditOutput
{
    public CreateOrEditPrescriptionDto Prescription { get; set; }
    public CreateOrEditPatientDto Patient { get; set; }
    public IList<MedicationDto> AllMedicines { get; set; }

}
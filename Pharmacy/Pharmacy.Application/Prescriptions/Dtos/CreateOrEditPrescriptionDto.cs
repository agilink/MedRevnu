using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace ATI.Pharmacy.Dtos;

public class CreateOrEditPrescriptionDto : EntityDto<int?>
{
    public CreateOrEditPrescriptionDto()
    {
        PrescriptionItems = new List<PrescriptionItemDto>();
    }
    public int PrescriptionID { get; set; }

    public int PatientID { get; set; }
    public int DoctorID { get; set; }
    public int DoctorDefaultFacilityId { get; set; }
    public string Notes { get; set; }
    public int DeliveryTypeId { get; set; }
    public int BillingTo { get; set; }
    public List<PrescriptionItemDto> PrescriptionItems { get; set; }
    public int? DosageRouteId { get; set; }


    //[StringLength(DoctorConsts.MaxSpecialtyLength, MinimumLength = DoctorConsts.MinSpecialtyLength)]
    //public string Specialty { get; set; }

    //[StringLength(DoctorConsts.MaxLicenseNumberLength, MinimumLength = DoctorConsts.MinLicenseNumberLength)]
    //public string LicenseNumber { get; set; }




}
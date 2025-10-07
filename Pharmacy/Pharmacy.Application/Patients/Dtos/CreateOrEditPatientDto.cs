using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using ATI.Pharmacy.Application.Patients;
using ATI.Pharmacy.Domain.Entities;
using ATI.Pharmacy.Application;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ATI.Pharmacy.Dtos;

public class CreateOrEditPatientDto : EntityDto<int?>
{
    public CreateOrEditPatientDto()
    {
        Prescriptions = new List<PrescriptionDto>();
    }
    public Nullable<DateTime> DateOfBirth { get; set; }
    public GenderEnum Gender { get; set; }
    public int? GenderId { get; set; }
    public string? EmergencyContactName { get; set; }


    public string? EmergencyContactPhone { get; set; }

    [Required]
    [StringLength(PatientConsts.MaxNameLength, MinimumLength = PatientConsts.MinNameLength)]
    public string Name { get; set; }

    [StringLength(PatientConsts.MaxSurnameLength, MinimumLength = PatientConsts.MinSurnameLength)]
    public string Surname { get; set; }

    public string EmailAddress { get; set; }
    [Required]
    [StringLength(PatientConsts.MaxPhoneNumberLength, MinimumLength = PatientConsts.MinPhoneNumberLength)]
    public string PhoneNumber { get; set; }

    public string InsuranceProvider { get; set; }


    public string? PolicyNumber { get; set; }

    public string? CoverageDetails { get; set; }

    //Address

    [Required]
    public string Address1 { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string ZipCode { get; set; }
    [Required]
    public int StateId { get; set; }
    [StringLength(PatientConsts.MaxFaxNoLength, MinimumLength = PatientConsts.MinFaxNoLength)]
    public string? FaxNo { get; set; }
    public List<SelectListItem>? Allergies { get; set; }

    public List<int>? AllergyId { get; set; }

    public string AllergyIds { get; set; }
    public string? PCN { get; set; }
    public string? Bin { get; set; }
    public string? Group { get; set; }

    public string? Other { get; set; }
    public List<PrescriptionDto> Prescriptions { get; set; }

    public int? DoctorId { get; set; }
}
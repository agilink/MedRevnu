using System;
using Abp.Application.Services.Dto;
using ATI.Admin.Application.Addresses.Dtos;
using ATI.Pharmacy.Application;
using ATI.Pharmacy.Application.Alergy.Dtos;
using ATI.Pharmacy.Application.Patients;

namespace ATI.Pharmacy.Dtos;

public class PatientDto : EntityDto
{
    public PatientDto()
    {
        PatientAllergies = new List<PatientAllergyDto>();
    }
    public string Name { get; set; }

    public string Surname { get; set; }

    public string EmailAddress { get; set; }

    public string PhoneNumber { get; set; }

    public GenderEnum Gender { get; set; }
    public int? GenderId { get; set; }

    public string GenderName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public UserDto User { get; set; }
    public AddressDto Address { get; set; }
    public List<PatientAllergyDto> PatientAllergies { get; set; }
}
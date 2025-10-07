using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using ATI.Authorization.Users;

namespace ATI.Pharmacy.Dtos;

public class CreateOrEditDoctorDto : EntityDto<int?>
{

    //[Required]
    public int DoctorID { get; set; }

    //[StringLength(DoctorConsts.MaxSpecialtyLength, MinimumLength = DoctorConsts.MinSpecialtyLength)]
    public string? Specialty { get; set; }

    [StringLength(DoctorConsts.MaxLicenseNumberLength, MinimumLength = DoctorConsts.MinLicenseNumberLength)]
    public string LicenseNumber { get; set; }
    public string? FacilityName { get; set; }

    public string SignatureFileID { get; set; }

    #region User info
    public string EmailAddress { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string? UserName { get; set; }
    #endregion


    public int[]? FacilityIds { get; set; }
}
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using ATI.Admin.Application.Companies.Dtos;
using ATI.Admin.Domain.Entities;
using ATI.Admin.Application.CompanyUser.Dtos;

namespace ATI.Pharmacy.Dtos;

public class CreateOrEditUserDto : EntityDto<int?>
{

    [Required]
    public int UserID { get; set; }

    [StringLength(UserConsts.MaxNameLength, MinimumLength = UserConsts.MinNameLength)]
    public string Name { get; set; }
    public string EmailAddress { get; set; }
    public string UserName { get; set; }


    [StringLength(UserConsts.MaxSurnameLength, MinimumLength = UserConsts.MinSurnameLength)]
    public string Surname { get; set; }

    public UserCompanyDto UserCompany { get; set; }

}
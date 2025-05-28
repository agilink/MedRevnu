using System;
using Abp.Application.Services.Dto;
using ATI.Admin.Application.Companies.Dtos;
using ATI.Admin.Application.Facilities.Dtos;
using ATI.Admin.Domain.Entities;
using ATI.Authorization.Users;

namespace ATI.Admin.Application.CompanyUser.Dtos;

public class UserCompanyDto : EntityDto
{
    public CompanyDto Company { get; set; }
    public FacilityDto Facility { get; set; }
}
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace ATI.Admin.Application.Companies.Dtos;

public class GetCompanyForEditOutput
{
    public CreateOrEditFacilityDto Company { get; set; }

}
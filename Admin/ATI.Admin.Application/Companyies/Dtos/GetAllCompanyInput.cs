using Abp.Application.Services.Dto;
using System;

namespace ATI.Admin.Application.Companies.Dtos;

public class GetAllCompanyInput : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }

}
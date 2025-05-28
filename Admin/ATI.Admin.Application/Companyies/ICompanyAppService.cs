using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Admin.Application.Companies.Dtos;
using ATI.Dto;

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using ATI.Admin.Domain.Entities;

namespace ATI.Admin.Application.Companies;

public interface ICompanyAppService : IApplicationService
{
    Task<PagedResultDto<GetCompanyForViewDto>> GetAll(GetAllCompanyInput input);

    Task<GetCompanyForViewDto> GetCompanyForView(int id);

    Task<GetCompanyForEditOutput> GetCompanyForEdit(EntityDto input);

    Task CreateOrEdit(CreateOrEditFacilityDto input);

    Task Delete(EntityDto input);
    Task<int> Insert(Company company);

}
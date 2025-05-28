using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using ATI.Admin.Application.Companies.Dtos;
using ATI.Admin.Domain.Entities;
using ATI.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace ATI.Admin.Application.Companies;

public class CompanyAppService : ATIAppServiceBase, ICompanyAppService
{
    private readonly IRepository<Company> _companyRepository;

    public CompanyAppService(IRepository<Company> companyRepository)
    {
        _companyRepository = companyRepository;

    }

    public virtual async Task<PagedResultDto<GetCompanyForViewDto>> GetAll(GetAllCompanyInput input)
    {

        var filteredCompany = _companyRepository.GetAll()
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.CompanyName.Contains(input.Filter));

        var pagedAndFilteredCompany = filteredCompany
            .OrderBy(input.Sorting ?? "id asc")
            .PageBy(input);

        var company = from o in pagedAndFilteredCompany
                      select new
                      {

                          Id = o.Id
                      };

        var totalCount = await filteredCompany.CountAsync();

        var dbList = await company.ToListAsync();
        var results = new List<GetCompanyForViewDto>();

        foreach (var o in dbList)
        {
            var res = new GetCompanyForViewDto()
            {
                Company = new CompanyDto
                {

                    Id = o.Id,
                }
            };

            results.Add(res);
        }

        return new PagedResultDto<GetCompanyForViewDto>(
            totalCount,
            results
        );

    }

    public virtual async Task<GetCompanyForViewDto> GetCompanyForView(int id)
    {
        var company = await _companyRepository.GetAsync(id);

        var output = new GetCompanyForViewDto { Company = ObjectMapper.Map<CompanyDto>(company) };

        return output;
    }

    public virtual async Task<GetCompanyForEditOutput> GetCompanyForEdit(EntityDto input)
    {
        var company = await _companyRepository.FirstOrDefaultAsync(input.Id);

        var output = new GetCompanyForEditOutput { Company = ObjectMapper.Map<CreateOrEditFacilityDto>(company) };

        return output;
    }

    public virtual async Task CreateOrEdit(CreateOrEditFacilityDto input)
    {
        if (input.Id == null)
        {
            await Create(input);
        }
        else
        {
            await Update(input);
        }
    }
    protected virtual async Task Create(CreateOrEditFacilityDto input)
    {
        var company = ObjectMapper.Map<Company>(input);

        if (AbpSession.TenantId != null)
        {
            // company.TenantId = (int)AbpSession.TenantId;
        }

        await _companyRepository.InsertAsync(company);

    }

    protected virtual async Task Update(CreateOrEditFacilityDto input)
    {
        var company = await _companyRepository.FirstOrDefaultAsync((int)input.Id);
        ObjectMapper.Map(input, company);

    }

    public virtual async Task Delete(EntityDto input)
    {
        await _companyRepository.DeleteAsync(input.Id);
    }

    public virtual async Task<int> Insert(Company company)
    {
        await _companyRepository.InsertAndGetIdAsync(company);
        return company.Id;
    }



}
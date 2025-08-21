using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using ATI.Admin.Domain.Entities;
using ATI.Authorization;
using ATI.Authorization.Users;
using ATI.MedRevnu.Application.LafayetteQuota.Dto;
using ATI.MedRevnu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ATI.MedRevnu.Application.LafayetteQuota
{
    [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Personnel)]
    public class PersonnelAppService : ApplicationService, IPersonnelAppService
    {
        private readonly IRepository<Personnel> _personnelRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<User, long> _userRepository;

        public PersonnelAppService(
            IRepository<Personnel> personnelRepository,
            IRepository<Company> companyRepository,
            IRepository<User, long> userRepository)
        {
            _personnelRepository = personnelRepository;
            _companyRepository = companyRepository;
            _userRepository = userRepository;
        }

        public async Task<PagedResultDto<GetPersonnelForViewDto>> GetAll(GetAllPersonnelInput input)
        {
            var filteredPersonnel = _personnelRepository.GetAll()
                .Include(x => x.Cases)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.FirstName.Contains(input.Filter) || 
                                                                        e.LastName.Contains(input.Filter) || 
                                                                        e.EmailAddress.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.FirstNameFilter), e => e.FirstName.Contains(input.FirstNameFilter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.LastNameFilter), e => e.LastName.Contains(input.LastNameFilter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.EmailAddressFilter), e => e.EmailAddress.Contains(input.EmailAddressFilter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter), e => e.Title.Contains(input.TitleFilter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.SpecialtyFilter), e => e.Specialty.Contains(input.SpecialtyFilter))
                .WhereIf(input.IsActiveFilter.HasValue, e => e.IsActive == input.IsActiveFilter)
                .WhereIf(input.CompanyIdFilter.HasValue, e => e.CompanyId == input.CompanyIdFilter);

            var pagedAndFilteredPersonnel = filteredPersonnel
                .OrderBy(input.Sorting ?? "FirstName asc")
                .PageBy(input);

            var personnel = from p in pagedAndFilteredPersonnel
                           join c in _companyRepository.GetAll() on p.CompanyId equals c.Id into companyJoin
                           from company in companyJoin.DefaultIfEmpty()
                           join u in _userRepository.GetAll() on p.UserId equals u.Id into userJoin
                           from user in userJoin.DefaultIfEmpty()
                           join creatorUser in _userRepository.GetAll() on p.CreatorUserId equals creatorUser.Id into creatorJoin
                           from creator in creatorJoin.DefaultIfEmpty()
                           join modifierUser in _userRepository.GetAll() on p.LastModifierUserId equals modifierUser.Id into modifierJoin
                           from modifier in modifierJoin.DefaultIfEmpty()
                           select new GetPersonnelForViewDto
                           {
                               Personnel = new PersonnelDto
                               {
                                   Id = p.Id,
                                   FirstName = p.FirstName,
                                   LastName = p.LastName,
                                   FullName = p.FirstName + " " + p.LastName,
                                   EmailAddress = p.EmailAddress,
                                   PhoneNumber = p.PhoneNumber,
                                   Title = p.Title,
                                   Specialty = p.Specialty,
                                   LicenseNumber = p.LicenseNumber,
                                   IsActive = p.IsActive,
                                   CompanyId = p.CompanyId,
                                   UserId = p.UserId,
                                   CreationTime = p.CreationTime,
                                   CreatorUserName = creator != null ? creator.UserName : "",
                                   LastModificationTime = p.LastModificationTime,
                                   LastModifierUserName = modifier != null ? modifier.UserName : ""
                               },
                               CompanyName = company != null ? company.CompanyName : "",
                               UserName = user != null ? user.UserName : ""
                           };

            var totalCount = await filteredPersonnel.CountAsync();

            return new PagedResultDto<GetPersonnelForViewDto>(
                totalCount,
                await personnel.ToListAsync()
            );
        }

        public async Task<GetPersonnelForViewDto> GetPersonnelForView(int id)
        {
            var personnel = await _personnelRepository.GetAll()
                .Include(x => x.Cases)
                .FirstOrDefaultAsync(x => x.Id == id);

            var company = personnel.CompanyId.HasValue 
                ? await _companyRepository.FirstOrDefaultAsync(personnel.CompanyId.Value)
                : null;

            var user = personnel.UserId.HasValue 
                ? await _userRepository.FirstOrDefaultAsync(personnel.UserId.Value)
                : null;

            var output = new GetPersonnelForViewDto
            {
                Personnel = ObjectMapper.Map<PersonnelDto>(personnel),
                CompanyName = company?.CompanyName ?? "",
                UserName = user?.UserName ?? ""
            };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Personnel_Create)]
        public async Task<GetPersonnelForEditOutput> GetPersonnelForEdit(EntityDto input)
        {
            var personnel = await _personnelRepository.FirstOrDefaultAsync(input.Id);

            var company = personnel.CompanyId.HasValue 
                ? await _companyRepository.FirstOrDefaultAsync(personnel.CompanyId.Value)
                : null;

            var user = personnel.UserId.HasValue 
                ? await _userRepository.FirstOrDefaultAsync(personnel.UserId.Value)
                : null;

            var output = new GetPersonnelForEditOutput
            {
                Personnel = ObjectMapper.Map<CreateOrEditPersonnelDto>(personnel),
                CompanyName = company?.CompanyName ?? "",
                UserName = user?.UserName ?? ""
            };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Personnel_Create, AppPermissions.Pages_Administration_LafayetteQuota_Personnel_Edit)]
        public async Task CreateOrEdit(CreateOrEditPersonnelDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Personnel_Create)]
        protected virtual async Task Create(CreateOrEditPersonnelDto input)
        {
            var personnel = ObjectMapper.Map<Personnel>(input);

            await _personnelRepository.InsertAsync(personnel);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Personnel_Edit)]
        protected virtual async Task Update(CreateOrEditPersonnelDto input)
        {
            var personnel = await _personnelRepository.GetAsync(input.Id.Value);
            ObjectMapper.Map(input, personnel);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Personnel_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _personnelRepository.DeleteAsync(input.Id);
        }

        public async Task<PagedResultDto<GetAllPersonnelForLookupTableOutput>> GetAllPersonnelForLookupTable(
            GetAllPersonnelForLookupTableInput input)
        {
            var query = _personnelRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => 
                    e.FirstName.Contains(input.Filter) || e.LastName.Contains(input.Filter))
                .Where(e => e.IsActive);

            var totalCount = await query.CountAsync();

            var personnelList = await query
                .OrderBy(input.Sorting ?? "firstName asc, lastName asc")
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = personnelList.Select(personnel => new GetAllPersonnelForLookupTableOutput
            {
                Id = personnel.Id.ToString(),
                DisplayName = personnel.FirstName + " " + personnel.LastName + 
                            (!string.IsNullOrWhiteSpace(personnel.Title) ? " - " + personnel.Title : "")
            }).ToList();

            return new PagedResultDto<GetAllPersonnelForLookupTableOutput>(
                totalCount,
                lookupTableDtoList
            );
        }
    }
}
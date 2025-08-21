using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
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
    [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Cases)]
    public class CaseAppService : ApplicationService, ICaseAppService
    {
        private readonly IRepository<Case> _caseRepository;
        private readonly IRepository<Personnel> _personnelRepository;
        private readonly IRepository<CaseProduct> _caseProductRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<User, long> _userRepository;

        public CaseAppService(
            IRepository<Case> caseRepository,
            IRepository<Personnel> personnelRepository,
            IRepository<CaseProduct> caseProductRepository,
            IRepository<Product> productRepository,
            IRepository<User, long> userRepository)
        {
            _caseRepository = caseRepository;
            _personnelRepository = personnelRepository;
            _caseProductRepository = caseProductRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<PagedResultDto<GetCaseForViewDto>> GetAll(GetAllCasesInput input)
        {
            var filteredCases = _caseRepository.GetAll()
                .Include(x => x.Doctor)
                .Include(x => x.CaseProducts)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.CaseNumber.Contains(input.Filter) || 
                                                                       e.Description.Contains(input.Filter) || 
                                                                       e.ProcedureType.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.CaseNumberFilter), e => e.CaseNumber.Contains(input.CaseNumberFilter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.ProcedureTypeFilter), e => e.ProcedureType.Contains(input.ProcedureTypeFilter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.StatusFilter), e => e.Status.Contains(input.StatusFilter))
                .WhereIf(input.DateFromFilter.HasValue, e => e.Date >= input.DateFromFilter.Value)
                .WhereIf(input.DateToFilter.HasValue, e => e.Date <= input.DateToFilter.Value)
                .WhereIf(input.DoctorIdFilter.HasValue, e => e.DoctorId == input.DoctorIdFilter)
                .WhereIf(input.HospitalIdFilter.HasValue, e => e.HospitalId == input.HospitalIdFilter)
                .WhereIf(input.FacilityIdFilter.HasValue, e => e.FacilityId == input.FacilityIdFilter);

            var pagedAndFilteredCases = filteredCases
                .OrderBy(input.Sorting ?? "Date desc")
                .PageBy(input);

            var cases = from c in pagedAndFilteredCases
                       join d in _personnelRepository.GetAll() on c.DoctorId equals d.Id into doctorJoin
                       from doctor in doctorJoin.DefaultIfEmpty()
                       join creatorUser in _userRepository.GetAll() on c.CreatorUserId equals creatorUser.Id into creatorJoin
                       from creator in creatorJoin.DefaultIfEmpty()
                       join modifierUser in _userRepository.GetAll() on c.LastModifierUserId equals modifierUser.Id into modifierJoin
                       from modifier in modifierJoin.DefaultIfEmpty()
                       select new GetCaseForViewDto
                       {
                           Case = new CaseDto
                           {
                               Id = c.Id,
                               Date = c.Date,
                               CaseNumber = c.CaseNumber,
                               Description = c.Description,
                               ProcedureType = c.ProcedureType,
                               Status = c.Status,
                               Notes = c.Notes,
                               DoctorId = c.DoctorId,
                               HospitalId = c.HospitalId,
                               FacilityId = c.FacilityId,
                               TotalRevenue = c.CaseProducts.Sum(cp => cp.Revenue),
                               ProductCount = c.CaseProducts.Count(),
                               CreationTime = c.CreationTime,
                               CreatorUserName = creator != null ? creator.UserName : "",
                               LastModificationTime = c.LastModificationTime,
                               LastModifierUserName = modifier != null ? modifier.UserName : ""
                           },
                           DoctorName = doctor != null ? doctor.FirstName + " " + doctor.LastName : "",
                           HospitalName = "", // TODO: Add hospital lookup when implemented
                           FacilityName = ""  // TODO: Add facility lookup when implemented
                       };

            var totalCount = await filteredCases.CountAsync();

            return new PagedResultDto<GetCaseForViewDto>(
                totalCount,
                await cases.ToListAsync()
            );
        }

        public async Task<GetCaseForViewDto> GetCaseForView(int id)
        {
            var caseEntity = await _caseRepository.GetAll()
                .Include(x => x.Doctor)
                .Include(x => x.CaseProducts)
                .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(x => x.Id == id);

            var doctor = caseEntity.DoctorId.HasValue 
                ? await _personnelRepository.FirstOrDefaultAsync(caseEntity.DoctorId.Value)
                : null;

            var caseProducts = caseEntity.CaseProducts.Select(cp => new CaseProductDto
            {
                Id = cp.Id,
                CaseId = cp.CaseId,
                ProductId = cp.ProductId,
                ProductName = cp.Product?.Name ?? "",
                Quantity = cp.Quantity,
                Revenue = cp.Revenue
            }).ToList();

            var output = new GetCaseForViewDto
            {
                Case = ObjectMapper.Map<CaseDto>(caseEntity),
                DoctorName = doctor != null ? doctor.FirstName + " " + doctor.LastName : "",
                HospitalName = "", // TODO: Add hospital lookup when implemented
                FacilityName = "", // TODO: Add facility lookup when implemented
                CaseProducts = caseProducts
            };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Cases_Create, AppPermissions.Pages_Administration_LafayetteQuota_Cases_Edit)]
        public async Task<GetCaseForEditOutput> GetCaseForEdit(EntityDto input)
        {
            var caseEntity = await _caseRepository.GetAll()
                .Include(x => x.Doctor)
                .Include(x => x.CaseProducts)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            var doctor = caseEntity.DoctorId.HasValue 
                ? await _personnelRepository.FirstOrDefaultAsync(caseEntity.DoctorId.Value)
                : null;

            var output = new GetCaseForEditOutput
            {
                Case = ObjectMapper.Map<CreateOrEditCaseDto>(caseEntity),
                DoctorName = doctor != null ? doctor.FirstName + " " + doctor.LastName : "",
                HospitalName = "", // TODO: Add hospital lookup when implemented
                FacilityName = ""  // TODO: Add facility lookup when implemented
            };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Cases_Create, AppPermissions.Pages_Administration_LafayetteQuota_Cases_Edit)]
        public async Task CreateOrEdit(CreateOrEditCaseDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Cases_Create)]
        protected virtual async Task Create(CreateOrEditCaseDto input)
        {
            var caseEntity = ObjectMapper.Map<Case>(input);

            await _caseRepository.InsertAsync(caseEntity);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Cases_Edit)]
        protected virtual async Task Update(CreateOrEditCaseDto input)
        {
            var caseEntity = await _caseRepository.GetAsync(input.Id.Value);
            ObjectMapper.Map(input, caseEntity);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Cases_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _caseRepository.DeleteAsync(input.Id);
        }

        public async Task<PagedResultDto<GetAllCasesForLookupTableOutput>> GetAllCasesForLookupTable(
            GetAllCasesForLookupTableInput input)
        {
            var query = _caseRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => 
                    e.CaseNumber.Contains(input.Filter) || e.Description.Contains(input.Filter))
                .Where(e => e.Status == "Active");

            var totalCount = await query.CountAsync();

            var casesList = await query
                .OrderBy(input.Sorting ?? "date desc, caseNumber asc")
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = casesList.Select(caseEntity => new GetAllCasesForLookupTableOutput
            {
                Id = caseEntity.Id.ToString(),
                DisplayName = caseEntity.CaseNumber + 
                            (!string.IsNullOrWhiteSpace(caseEntity.Description) ? " - " + caseEntity.Description : "")
            }).ToList();

            return new PagedResultDto<GetAllCasesForLookupTableOutput>(
                totalCount,
                lookupTableDtoList
            );
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
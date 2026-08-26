using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using ATI.Admin.Domain.Entities;
using ATI.Revenue.Application.Hospitals.Dtos;
using ATI.Revenue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Hospitals
{
    /// <summary>
    /// Manages the hospital list.
    /// </summary>
    /// <remarks>
    /// Hospitals are ADM.Facility rows, referenced by revenue transactions, product
    /// quotas, contracted prices and physicians. Like the physician roster there was no
    /// UI for them anywhere, so the list could only be changed by running a seed script.
    ///
    /// Name, status and owning company are editable. Every hospital must belong to a
    /// company - one can be picked or created inline - because a facility with no company
    /// is invisible to anything that groups by company. A facility's address remains a
    /// separate Admin entity with its own screen to come.
    /// </remarks>
    public class HospitalsAppService : ApplicationService, IHospitalsAppService
    {
        private readonly IRepository<Facility, int> _facilityRepository;
        private readonly IRepository<Company, int> _companyRepository;
        private readonly IRepository<Personnel, int> _personnelRepository;
        private readonly IRepository<ProcedureTransaction, int> _procedureTransactionRepository;
        private readonly IRepository<HospitalProductPrice, int> _hospitalProductPriceRepository;
        private readonly IRepository<ProductQuota, int> _productQuotaRepository;

        public HospitalsAppService(
            IRepository<Facility, int> facilityRepository,
            IRepository<Company, int> companyRepository,
            IRepository<Personnel, int> personnelRepository,
            IRepository<ProcedureTransaction, int> procedureTransactionRepository,
            IRepository<HospitalProductPrice, int> hospitalProductPriceRepository,
            IRepository<ProductQuota, int> productQuotaRepository)
        {
            _facilityRepository = facilityRepository;
            _companyRepository = companyRepository;
            _personnelRepository = personnelRepository;
            _procedureTransactionRepository = procedureTransactionRepository;
            _hospitalProductPriceRepository = hospitalProductPriceRepository;
            _productQuotaRepository = productQuotaRepository;
        }

        public async Task<PagedResultDto<HospitalDto>> GetAll(GetAllHospitalsInput input)
        {
            var baseQuery = _facilityRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                var filter = input.Filter.Trim();
                baseQuery = baseQuery.Where(f => f.FacilityName != null && f.FacilityName.Contains(filter));
            }

            var totalCount = await baseQuery.CountAsync();

            var sortedQuery = string.IsNullOrWhiteSpace(input.Sorting)
                ? baseQuery.OrderBy(f => f.FacilityName)
                : baseQuery.OrderBy(input.Sorting);

            var dtos = await Project(sortedQuery.PageBy(input)).ToListAsync();

            return new PagedResultDto<HospitalDto>(totalCount, dtos);
        }

        public async Task<GetHospitalForViewDto> GetHospitalForView(int id)
        {
            var dto = await Project(_facilityRepository.GetAll().Where(f => f.Id == id)).FirstOrDefaultAsync();

            if (dto == null)
                throw new UserFriendlyException("Hospital not found");

            return new GetHospitalForViewDto { Hospital = dto };
        }

        public async Task<GetHospitalForEditOutput> GetHospitalForEdit(EntityDto<int> input)
        {
            var entity = await _facilityRepository.GetAll()
                .Include(f => f.Company)
                .FirstOrDefaultAsync(f => f.Id == input.Id);

            if (entity == null)
                throw new UserFriendlyException("Hospital not found");

            return new GetHospitalForEditOutput
            {
                Hospital = new CreateOrEditHospitalDto
                {
                    Id = entity.Id,
                    HospitalName = entity.FacilityName,
                    FacilityStatusId = entity.FacilityStatusId,
                    CompanyId = entity.Company != null ? (int?)entity.Company.Id : null
                }
            };
        }

        public async Task<HospitalDto> CreateOrEdit(CreateOrEditHospitalDto input)
        {
            var name = input.HospitalName?.Trim();

            // Hospital names are how the client identifies them on every report, so a
            // duplicate would make two rows indistinguishable.
            var duplicateExists = await _facilityRepository.GetAll()
                .AnyAsync(f => f.Id != input.Id && f.FacilityName == name);

            if (duplicateExists)
            {
                throw new UserFriendlyException($"A hospital named \"{name}\" already exists.");
            }

            var company = await ResolveCompany(input);

            var entity = input.Id == 0
                ? new Facility()
                : await _facilityRepository.GetAll().Include(f => f.Company)
                    .FirstAsync(f => f.Id == input.Id);

            entity.FacilityName = name;
            entity.FacilityStatusId = input.FacilityStatusId;

            // Facility's company link is an EF shadow foreign key - there is no CompanyId
            // property on the entity - so it is set through the navigation.
            entity.Company = company;

            int id;
            if (input.Id == 0)
            {
                id = await _facilityRepository.InsertAndGetIdAsync(entity);
            }
            else
            {
                await _facilityRepository.UpdateAsync(entity);
                id = entity.Id;
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return await Project(_facilityRepository.GetAll().Where(f => f.Id == id)).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Returns the company this hospital belongs to, creating it when the user typed a
        /// new name.
        /// </summary>
        /// <remarks>
        /// A hospital with no company is invisible to anything that groups by company, so
        /// one is required. Naming a company that already exists reuses it rather than
        /// creating a duplicate.
        /// </remarks>
        private async Task<Company> ResolveCompany(CreateOrEditHospitalDto input)
        {
            var newName = input.NewCompanyName?.Trim();

            if (!string.IsNullOrWhiteSpace(newName))
            {
                var existing = await _companyRepository.GetAll()
                    .FirstOrDefaultAsync(c => c.CompanyName == newName);

                if (existing != null)
                {
                    return existing;
                }

                var created = new Company { CompanyName = newName };
                created.Id = await _companyRepository.InsertAndGetIdAsync(created);
                await CurrentUnitOfWork.SaveChangesAsync();
                return created;
            }

            if (input.CompanyId.HasValue)
            {
                var selected = await _companyRepository.GetAll()
                    .FirstOrDefaultAsync(c => c.Id == input.CompanyId.Value);

                if (selected == null)
                {
                    throw new UserFriendlyException("The selected company no longer exists.");
                }

                return selected;
            }

            throw new UserFriendlyException(
                "A company is required",
                "Choose the company this hospital belongs to, or type a new company name to create one.");
        }

        public async Task Delete(EntityDto<int> input)
        {
            // Refuse while anything still points at it. Facility is soft-deleted, so the
            // rows would survive, but they would silently drop out of every hospital
            // filter and quota comparison with no indication why.
            var transactions = await _procedureTransactionRepository.CountAsync(t => t.HospitalId == input.Id);
            var quotas = await _productQuotaRepository.CountAsync(q => q.HospitalId == input.Id);
            var prices = await _hospitalProductPriceRepository.CountAsync(p => p.HospitalId == input.Id);
            var physicians = await _personnelRepository.CountAsync(p => p.FacilityId == input.Id);

            if (transactions + quotas + prices + physicians > 0)
            {
                throw new UserFriendlyException(
                    "This hospital is still in use",
                    $"It has {transactions} transaction(s), {quotas} quota(s), {prices} contracted price(s) " +
                    $"and {physicians} physician(s) attached. Move or remove those first.");
            }

            await _facilityRepository.DeleteAsync(input.Id);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private IQueryable<HospitalDto> Project(IQueryable<Facility> query)
        {
            return query.Select(f => new HospitalDto
            {
                Id = f.Id,
                HospitalName = f.FacilityName ?? "",
                FacilityStatusId = f.FacilityStatusId,
                CompanyId = f.Company != null ? (int?)f.Company.Id : null,
                CompanyName = f.Company != null ? (f.Company.CompanyName ?? "") : "",
                PhysicianCount = _personnelRepository.GetAll().Count(p => p.FacilityId == f.Id),
                ProductPriceCount = _hospitalProductPriceRepository.GetAll().Count(p => p.HospitalId == f.Id)
            });
        }
    }
}

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
    /// Only the name and status are editable here. A facility's address and owning
    /// company are separate entities in the Admin module with their own screens to come,
    /// and inventing a partial editor for them would risk writing half a record.
    /// </remarks>
    public class HospitalsAppService : ApplicationService, IHospitalsAppService
    {
        private readonly IRepository<Facility, int> _facilityRepository;
        private readonly IRepository<Personnel, int> _personnelRepository;
        private readonly IRepository<ProcedureTransaction, int> _procedureTransactionRepository;
        private readonly IRepository<HospitalProductPrice, int> _hospitalProductPriceRepository;
        private readonly IRepository<ProductQuota, int> _productQuotaRepository;

        public HospitalsAppService(
            IRepository<Facility, int> facilityRepository,
            IRepository<Personnel, int> personnelRepository,
            IRepository<ProcedureTransaction, int> procedureTransactionRepository,
            IRepository<HospitalProductPrice, int> hospitalProductPriceRepository,
            IRepository<ProductQuota, int> productQuotaRepository)
        {
            _facilityRepository = facilityRepository;
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
            var entity = await _facilityRepository.GetAll().FirstOrDefaultAsync(f => f.Id == input.Id);

            if (entity == null)
                throw new UserFriendlyException("Hospital not found");

            return new GetHospitalForEditOutput
            {
                Hospital = new CreateOrEditHospitalDto
                {
                    Id = entity.Id,
                    HospitalName = entity.FacilityName,
                    FacilityStatusId = entity.FacilityStatusId
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

            var entity = input.Id == 0
                ? new Facility()
                : await _facilityRepository.GetAsync(input.Id);

            entity.FacilityName = name;
            entity.FacilityStatusId = input.FacilityStatusId;

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
                CompanyName = f.Company != null ? (f.Company.CompanyName ?? "") : "",
                PhysicianCount = _personnelRepository.GetAll().Count(p => p.FacilityId == f.Id),
                ProductPriceCount = _hospitalProductPriceRepository.GetAll().Count(p => p.HospitalId == f.Id)
            });
        }
    }
}

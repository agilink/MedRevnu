using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using ATI.Revenue.Application.ProcedureQuotas.Dtos;
using ATI.Revenue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.ProcedureQuotas
{
    public class ProcedureQuotasAppService : ApplicationService, IProcedureQuotasAppService
    {
        private readonly IRepository<ProcedureQuota, int> _procedureQuotaRepository;
        private readonly IRepository<ProcedureType, int> _procedureTypeRepository;

        public ProcedureQuotasAppService(
            IRepository<ProcedureQuota, int> procedureQuotaRepository,
            IRepository<ProcedureType, int> procedureTypeRepository)
        {
            _procedureQuotaRepository = procedureQuotaRepository;
            _procedureTypeRepository = procedureTypeRepository;
        }

        public async Task<PagedResultDto<ProcedureQuotaDto>> GetAll(GetAllProcedureQuotasInput input)
        {
            var query = CreateFilteredQuery(input);

            var totalCount = await query.CountAsync();

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await query
                .Include(pq => pq.ProcedureType)
                .ToListAsync();

            var dtos = entities.Select(MapToDto).ToList();

            return new PagedResultDto<ProcedureQuotaDto>(totalCount, dtos);
        }

        public async Task<GetProcedureQuotaForViewDto> GetProcedureQuotaForView(int id)
        {
            var entity = await _procedureQuotaRepository.GetAll()
                .Include(pq => pq.ProcedureType)
                .FirstOrDefaultAsync(pq => pq.Id == id);

            if (entity == null)
            {
                throw new UserFriendlyException("Procedure Quota not found");
            }

            var dto = MapToDto(entity);
            return new GetProcedureQuotaForViewDto { ProcedureQuota = dto };
        }

        public async Task<GetProcedureQuotaForEditOutput> GetProcedureQuotaForEdit(EntityDto<int> input)
        {
            var entity = await _procedureQuotaRepository.GetAsync(input.Id);
            var editDto = ObjectMapper.Map<CreateOrEditProcedureQuotaDto>(entity);

            return new GetProcedureQuotaForEditOutput
            {
                ProcedureQuota = editDto
            };
        }

        public async Task CreateOrEdit(CreateOrEditProcedureQuotaDto input)
        {
            if (input.Id == 0)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        protected virtual async Task Create(CreateOrEditProcedureQuotaDto input)
        {
            // Validate date range
            if (input.EndDate <= input.StartDate)
            {
                throw new UserFriendlyException("End date must be after start date.");
            }

            // Check for overlapping quotas
            var hasOverlap = await _procedureQuotaRepository.GetAll()
                .AnyAsync(pq =>
                    pq.ProcedureTypeId == input.ProcedureTypeId &&
                    pq.FacilityId == input.FacilityId &&
                    ((input.StartDate >= pq.StartDate && input.StartDate < pq.EndDate) ||
                     (input.EndDate > pq.StartDate && input.EndDate <= pq.EndDate) ||
                     (input.StartDate <= pq.StartDate && input.EndDate >= pq.EndDate)));

            if (hasOverlap)
            {
                throw new UserFriendlyException("A quota already exists for this procedure type and date range.");
            }

            var entity = ObjectMapper.Map<ProcedureQuota>(input);
            await _procedureQuotaRepository.InsertAsync(entity);
        }

        protected virtual async Task Update(CreateOrEditProcedureQuotaDto input)
        {
            // Validate date range
            if (input.EndDate <= input.StartDate)
            {
                throw new UserFriendlyException("End date must be after start date.");
            }

            // Check for overlapping quotas (excluding current entity)
            var hasOverlap = await _procedureQuotaRepository.GetAll()
                .AnyAsync(pq =>
                    pq.Id != input.Id &&
                    pq.ProcedureTypeId == input.ProcedureTypeId &&
                    pq.FacilityId == input.FacilityId &&
                    ((input.StartDate >= pq.StartDate && input.StartDate < pq.EndDate) ||
                     (input.EndDate > pq.StartDate && input.EndDate <= pq.EndDate) ||
                     (input.StartDate <= pq.StartDate && input.EndDate >= pq.EndDate)));

            if (hasOverlap)
            {
                throw new UserFriendlyException("A quota already exists for this procedure type and date range.");
            }

            var entity = await _procedureQuotaRepository.GetAsync(input.Id);
            ObjectMapper.Map(input, entity);
            await _procedureQuotaRepository.UpdateAsync(entity);
        }

        public async Task Delete(EntityDto<int> input)
        {
            await _procedureQuotaRepository.DeleteAsync(input.Id);
        }

        public async Task<ProcedureQuotaDto> GetQuotaForProcedure(int procedureTypeId, DateTime date, int? facilityId = null)
        {
            var entity = await _procedureQuotaRepository.GetAll()
                .Include(pq => pq.ProcedureType)
                .Where(pq =>
                    pq.ProcedureTypeId == procedureTypeId &&
                    pq.FacilityId == facilityId &&
                    date >= pq.StartDate &&
                    date <= pq.EndDate)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                // Try to get a company-wide quota (no facility specified)
                entity = await _procedureQuotaRepository.GetAll()
                    .Include(pq => pq.ProcedureType)
                    .Where(pq =>
                        pq.ProcedureTypeId == procedureTypeId &&
                        pq.FacilityId == null &&
                        date >= pq.StartDate &&
                        date <= pq.EndDate)
                    .FirstOrDefaultAsync();
            }

            return entity != null ? MapToDto(entity) : null;
        }

        private IQueryable<ProcedureQuota> CreateFilteredQuery(GetAllProcedureQuotasInput input)
        {
            return _procedureQuotaRepository.GetAll()
                .WhereIf(input.ProcedureTypeIdFilter.HasValue,
                    e => e.ProcedureTypeId == input.ProcedureTypeIdFilter.Value)
                .WhereIf(input.FacilityIdFilter.HasValue,
                    e => e.FacilityId == input.FacilityIdFilter.Value)
                .WhereIf(input.QuotaPeriodFilter.HasValue,
                    e => e.QuotaPeriod == input.QuotaPeriodFilter.Value)
                .WhereIf(input.StartDateFilter.HasValue,
                    e => e.StartDate >= input.StartDateFilter.Value)
                .WhereIf(input.EndDateFilter.HasValue,
                    e => e.EndDate <= input.EndDateFilter.Value);
        }

        private IQueryable<ProcedureQuota> ApplySorting(IQueryable<ProcedureQuota> query, GetAllProcedureQuotasInput input)
        {
            var sortInput = input.Sorting ?? "StartDate DESC";
            return query.OrderBy(sortInput);
        }

        private IQueryable<ProcedureQuota> ApplyPaging(IQueryable<ProcedureQuota> query, GetAllProcedureQuotasInput input)
        {
            return query.Skip(input.SkipCount).Take(input.MaxResultCount);
        }

        private ProcedureQuotaDto MapToDto(ProcedureQuota entity)
        {
            var dto = ObjectMapper.Map<ProcedureQuotaDto>(entity);
            dto.ProcedureTypeName = entity.ProcedureType?.Name;
            dto.QuotaPeriodName = entity.QuotaPeriod.ToString();
            return dto;
        }
    }
}

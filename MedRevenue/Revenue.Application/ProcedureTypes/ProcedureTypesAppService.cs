using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using ATI.Revenue.Application.ProcedureTypes.Dtos;
using ATI.Revenue.Domain.Entities;
using ATI.Revenue.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.ProcedureTypes
{
    public class ProcedureTypesAppService : ApplicationService, IProcedureTypesAppService
    {
        private readonly IRepository<ProcedureType, int> _procedureTypeRepository;

        public ProcedureTypesAppService(IRepository<ProcedureType, int> procedureTypeRepository)
        {
            _procedureTypeRepository = procedureTypeRepository;
        }

        public async Task<PagedResultDto<ProcedureTypeDto>> GetAll(GetAllProcedureTypesInput input)
        {
            var query = CreateFilteredQuery(input);

            var totalCount = await query.CountAsync();

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await query.ToListAsync();
            var dtos = entities.Select(MapToDto).ToList();

            return new PagedResultDto<ProcedureTypeDto>(totalCount, dtos);
        }

        public async Task<GetProcedureTypeForViewDto> GetProcedureTypeForView(int id)
        {
            var entity = await _procedureTypeRepository.GetAsync(id);

            if (entity == null)
            {
                throw new UserFriendlyException("Procedure Type not found");
            }

            var dto = MapToDto(entity);
            return new GetProcedureTypeForViewDto { ProcedureType = dto };
        }

        public async Task<GetProcedureTypeForEditOutput> GetProcedureTypeForEdit(EntityDto<int> input)
        {
            var entity = await _procedureTypeRepository.GetAsync(input.Id);
            var editDto = ObjectMapper.Map<CreateOrEditProcedureTypeDto>(entity);

            return new GetProcedureTypeForEditOutput
            {
                ProcedureType = editDto
            };
        }

        public async Task CreateOrEdit(CreateOrEditProcedureTypeDto input)
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

        protected virtual async Task Create(CreateOrEditProcedureTypeDto input)
        {
            // Check if code already exists
            var exists = await _procedureTypeRepository.GetAll()
                .AnyAsync(pt => pt.Code == input.Code);

            if (exists)
            {
                throw new UserFriendlyException("A procedure type with this code already exists.");
            }

            var entity = ObjectMapper.Map<ProcedureType>(input);
            await _procedureTypeRepository.InsertAsync(entity);
        }

        protected virtual async Task Update(CreateOrEditProcedureTypeDto input)
        {
            // Check if code already exists for a different entity
            var exists = await _procedureTypeRepository.GetAll()
                .AnyAsync(pt => pt.Code == input.Code && pt.Id != input.Id);

            if (exists)
            {
                throw new UserFriendlyException("A procedure type with this code already exists.");
            }

            var entity = await _procedureTypeRepository.GetAsync(input.Id);
            ObjectMapper.Map(input, entity);
            await _procedureTypeRepository.UpdateAsync(entity);
        }

        public async Task Delete(EntityDto<int> input)
        {
            await _procedureTypeRepository.DeleteAsync(input.Id);
        }

        public async Task<List<ProcedureTypeDto>> GetAllActiveProcedureTypes()
        {
            var entities = await _procedureTypeRepository.GetAll()
                .Where(pt => pt.IsActive)
                .OrderBy(pt => pt.DisplayOrder)
                .ToListAsync();

            return entities.Select(MapToDto).ToList();
        }

        public async Task<List<ProcedureTypeDto>> GetProcedureTypesByCategory(CategoryGroup categoryGroup)
        {
            var entities = await _procedureTypeRepository.GetAll()
                .Where(pt => pt.CategoryGroup == categoryGroup && pt.IsActive)
                .OrderBy(pt => pt.DisplayOrder)
                .ToListAsync();

            return entities.Select(MapToDto).ToList();
        }

        private IQueryable<ProcedureType> CreateFilteredQuery(GetAllProcedureTypesInput input)
        {
            return _procedureTypeRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => e.Name.Contains(input.Filter) ||
                         e.Code.Contains(input.Filter) ||
                         e.Description.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),
                    e => e.Name.Contains(input.NameFilter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.CodeFilter),
                    e => e.Code.Contains(input.CodeFilter))
                .WhereIf(input.CategoryGroupFilter.HasValue,
                    e => e.CategoryGroup == input.CategoryGroupFilter.Value)
                .WhereIf(input.IsActiveFilter.HasValue,
                    e => e.IsActive == input.IsActiveFilter.Value);
        }

        private IQueryable<ProcedureType> ApplySorting(IQueryable<ProcedureType> query, GetAllProcedureTypesInput input)
        {
            var sortInput = input.Sorting ?? "DisplayOrder ASC";
            return query.OrderBy(sortInput);
        }

        private IQueryable<ProcedureType> ApplyPaging(IQueryable<ProcedureType> query, GetAllProcedureTypesInput input)
        {
            return query.Skip(input.SkipCount).Take(input.MaxResultCount);
        }

        private ProcedureTypeDto MapToDto(ProcedureType entity)
        {
            var dto = ObjectMapper.Map<ProcedureTypeDto>(entity);
            dto.CategoryGroupName = entity.CategoryGroup.ToString();
            return dto;
        }
    }
}

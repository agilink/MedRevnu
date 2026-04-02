using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using ATI.Admin.Domain.Entities;
using ATI.Revenue.Application.HospitalProductPrices.Dtos;
using ATI.Revenue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.HospitalProductPrices
{
    public class HospitalProductPricesAppService : ApplicationService, IHospitalProductPricesAppService
    {
        private readonly IRepository<HospitalProductPrice, int> _hospitalProductPriceRepository;
        private readonly IRepository<Facility, int> _facilityRepository;
        private readonly IRepository<Product, int> _productRepository;

        public HospitalProductPricesAppService(
            IRepository<HospitalProductPrice, int> hospitalProductPriceRepository,
            IRepository<Facility, int> facilityRepository,
            IRepository<Product, int> productRepository)
        {
            _hospitalProductPriceRepository = hospitalProductPriceRepository;
            _facilityRepository = facilityRepository;
            _productRepository = productRepository;
        }

        public async Task<PagedResultDto<HospitalProductPriceDto>> GetAll(GetAllHospitalProductPricesInput input)
        {
            var query = CreateFilteredQuery(input)
                .Include(hpp => hpp.Hospital)
                .Include(hpp => hpp.Product)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);
            else
                query = query.OrderBy(hpp => hpp.Hospital.FacilityName).ThenBy(hpp => hpp.ProductCode);

            query = query.PageBy(input);

            var entities = await query.ToListAsync();
            var dtos = entities.Select(MapToDto).ToList();

            return new PagedResultDto<HospitalProductPriceDto>(totalCount, dtos);
        }

        public async Task<GetHospitalProductPriceForViewDto> GetHospitalProductPriceForView(int id)
        {
            var entity = await _hospitalProductPriceRepository
                .GetAll()
                .Include(hpp => hpp.Hospital)
                .Include(hpp => hpp.Product)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entity == null)
                throw new UserFriendlyException("Hospital Product Price not found");

            return new GetHospitalProductPriceForViewDto { HospitalProductPrice = MapToDto(entity) };
        }

        public async Task<GetHospitalProductPriceForEditOutput> GetHospitalProductPriceForEdit(EntityDto<int> input)
        {
            var entity = await _hospitalProductPriceRepository
                .GetAll()
                .FirstOrDefaultAsync(e => e.Id == input.Id);

            if (entity == null)
                throw new UserFriendlyException("Hospital Product Price not found");

            return new GetHospitalProductPriceForEditOutput
            {
                HospitalProductPrice = new CreateOrEditHospitalProductPriceDto
                {
                    Id = entity.Id,
                    HospitalId = entity.HospitalId,
                    ProductId = entity.ProductId,
                    ProductCode = entity.ProductCode,
                    UnitPrice = entity.UnitPrice,
                    EffectiveDate = entity.EffectiveDate,
                    IsActive = entity.IsActive
                }
            };
        }

        public async Task<HospitalProductPriceDto> CreateOrEdit(CreateOrEditHospitalProductPriceDto input)
        {
            if (input.Id == 0)
                return await Create(input);
            else
                return await Update(input);
        }

        private async Task<HospitalProductPriceDto> Create(CreateOrEditHospitalProductPriceDto input)
        {
            var entity = new HospitalProductPrice
            {
                HospitalId = input.HospitalId,
                ProductId = input.ProductId,
                ProductCode = input.ProductCode,
                UnitPrice = input.UnitPrice,
                EffectiveDate = input.EffectiveDate,
                IsActive = input.IsActive
            };

            var id = await _hospitalProductPriceRepository.InsertAndGetIdAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            entity = await _hospitalProductPriceRepository
                .GetAll()
                .Include(hpp => hpp.Hospital)
                .Include(hpp => hpp.Product)
                .FirstOrDefaultAsync(e => e.Id == id);

            return MapToDto(entity);
        }

        private async Task<HospitalProductPriceDto> Update(CreateOrEditHospitalProductPriceDto input)
        {
            var entity = await _hospitalProductPriceRepository.GetAsync(input.Id);

            entity.HospitalId = input.HospitalId;
            entity.ProductId = input.ProductId;
            entity.ProductCode = input.ProductCode;
            entity.UnitPrice = input.UnitPrice;
            entity.EffectiveDate = input.EffectiveDate;
            entity.IsActive = input.IsActive;

            await _hospitalProductPriceRepository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            entity = await _hospitalProductPriceRepository
                .GetAll()
                .Include(hpp => hpp.Hospital)
                .Include(hpp => hpp.Product)
                .FirstOrDefaultAsync(e => e.Id == entity.Id);

            return MapToDto(entity);
        }

        public async Task Delete(EntityDto<int> input)
        {
            await _hospitalProductPriceRepository.DeleteAsync(input.Id);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private IQueryable<HospitalProductPrice> CreateFilteredQuery(GetAllHospitalProductPricesInput input)
        {
            var query = _hospitalProductPriceRepository.GetAll();

            if (input.HospitalIdFilter.HasValue)
                query = query.Where(hpp => hpp.HospitalId == input.HospitalIdFilter.Value);

            if (input.ProductIdFilter.HasValue)
                query = query.Where(hpp => hpp.ProductId == input.ProductIdFilter.Value);

            if (input.IsActiveFilter.HasValue)
                query = query.Where(hpp => hpp.IsActive == input.IsActiveFilter.Value);

            if (!string.IsNullOrWhiteSpace(input.Filter))
                query = query.Where(hpp =>
                    hpp.Hospital.FacilityName.Contains(input.Filter) ||
                    hpp.Product.Name.Contains(input.Filter) ||
                    (hpp.ProductCode != null && hpp.ProductCode.Contains(input.Filter)));

            return query;
        }

        private static HospitalProductPriceDto MapToDto(HospitalProductPrice entity)
        {
            return new HospitalProductPriceDto
            {
                Id = entity.Id,
                HospitalId = entity.HospitalId,
                HospitalName = entity.Hospital?.FacilityName ?? "",
                ProductId = entity.ProductId,
                ProductName = entity.Product?.Name ?? "",
                ProductCode = entity.ProductCode,
                UnitPrice = entity.UnitPrice,
                EffectiveDate = entity.EffectiveDate,
                IsActive = entity.IsActive
            };
        }
    }
}

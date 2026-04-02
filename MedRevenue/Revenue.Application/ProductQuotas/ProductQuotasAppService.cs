using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using ATI.Admin.Domain.Entities;
using ATI.Revenue.Application.ProductQuotas.Dtos;
using ATI.Revenue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.ProductQuotas
{
    public class ProductQuotasAppService : ApplicationService, IProductQuotasAppService
    {
        private readonly IRepository<ProductQuota, int> _productQuotaRepository;
        private readonly IRepository<Facility, int> _facilityRepository;
        private readonly IRepository<ProductCategory, int> _productCategoryRepository;
        private readonly IRepository<Product, int> _productRepository;

        public ProductQuotasAppService(
            IRepository<ProductQuota, int> productQuotaRepository,
            IRepository<Facility, int> facilityRepository,
            IRepository<ProductCategory, int> productCategoryRepository,
            IRepository<Product, int> productRepository)
        {
            _productQuotaRepository = productQuotaRepository;
            _facilityRepository = facilityRepository;
            _productCategoryRepository = productCategoryRepository;
            _productRepository = productRepository;
        }

        public async Task<PagedResultDto<ProductQuotaDto>> GetAll(GetAllProductQuotasInput input)
        {
            var query = CreateFilteredQuery(input)
                .Include(pq => pq.Hospital)
                .Include(pq => pq.ProductCategory)
                .Include(pq => pq.Product)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                query = query.OrderBy(input.Sorting);
            }
            else
            {
                query = query.OrderByDescending(pq => pq.PeriodYear)
                             .ThenByDescending(pq => pq.PeriodMonth);
            }

            // Apply paging
            query = query.PageBy(input);

            var entities = await query.ToListAsync();
            var dtos = entities.Select(entity => new ProductQuotaDto
            {
                Id = entity.Id,
                PeriodYear = entity.PeriodYear,
                PeriodMonth = entity.PeriodMonth,
                HospitalId = entity.HospitalId,
                HospitalName = entity.Hospital?.FacilityName ?? "",
                ProductCategoryId = entity.ProductCategoryId,
                ProductCategoryName = entity.ProductCategory?.Name ?? "",
                ProductId = entity.ProductId,
                ProductName = entity.Product?.Name ?? "",
                TargetAmount = entity.TargetAmount,
                TargetUnits = entity.TargetUnits
            }).ToList();

            return new PagedResultDto<ProductQuotaDto>(totalCount, dtos);
        }

        public async Task<GetProductQuotaForViewDto> GetProductQuotaForView(int id)
        {
            var entity = await _productQuotaRepository
                .GetAll()
                .Include(pq => pq.Hospital)
                .Include(pq => pq.ProductCategory)
                .Include(pq => pq.Product)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entity == null)
            {
                throw new UserFriendlyException("Product Quota not found");
            }

            var dto = new ProductQuotaDto
            {
                Id = entity.Id,
                PeriodYear = entity.PeriodYear,
                PeriodMonth = entity.PeriodMonth,
                HospitalId = entity.HospitalId,
                HospitalName = entity.Hospital?.FacilityName ?? "",
                ProductCategoryId = entity.ProductCategoryId,
                ProductCategoryName = entity.ProductCategory?.Name ?? "",
                ProductId = entity.ProductId,
                ProductName = entity.Product?.Name ?? "",
                TargetAmount = entity.TargetAmount,
                TargetUnits = entity.TargetUnits
            };

            return new GetProductQuotaForViewDto { ProductQuota = dto };
        }

        public async Task<GetProductQuotaForEditOutput> GetProductQuotaForEdit(EntityDto<int> input)
        {
            var entity = await _productQuotaRepository
                .GetAll()
                .FirstOrDefaultAsync(e => e.Id == input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException("Product Quota not found");
            }

            var editDto = new CreateOrEditProductQuotaDto
            {
                Id = entity.Id,
                PeriodYear = entity.PeriodYear,
                PeriodMonth = entity.PeriodMonth,
                HospitalId = entity.HospitalId,
                ProductCategoryId = entity.ProductCategoryId,
                ProductId = entity.ProductId,
                TargetAmount = entity.TargetAmount,
                TargetUnits = entity.TargetUnits
            };

            return new GetProductQuotaForEditOutput
            {
                ProductQuota = editDto
            };
        }

        public async Task<ProductQuotaDto> CreateOrEdit(CreateOrEditProductQuotaDto input)
        {
            if (input.Id == 0)
            {
                return await Create(input);
            }
            else
            {
                return await Update(input);
            }
        }

        private async Task<ProductQuotaDto> Create(CreateOrEditProductQuotaDto input)
        {
            var entity = new ProductQuota
            {
                PeriodYear = input.PeriodYear,
                PeriodMonth = input.PeriodMonth,
                HospitalId = input.HospitalId,
                ProductCategoryId = input.ProductCategoryId,
                ProductId = input.ProductId,
                TargetAmount = input.TargetAmount,
                TargetUnits = input.TargetUnits
            };

            var id = await _productQuotaRepository.InsertAndGetIdAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Load navigation properties for return DTO
            entity = await _productQuotaRepository
                .GetAll()
                .Include(pq => pq.Hospital)
                .Include(pq => pq.ProductCategory)
                .Include(pq => pq.Product)
                .FirstOrDefaultAsync(e => e.Id == id);

            return new ProductQuotaDto
            {
                Id = entity.Id,
                PeriodYear = entity.PeriodYear,
                PeriodMonth = entity.PeriodMonth,
                HospitalId = entity.HospitalId,
                HospitalName = entity.Hospital?.FacilityName ?? "",
                ProductCategoryId = entity.ProductCategoryId,
                ProductCategoryName = entity.ProductCategory?.Name ?? "",
                ProductId = entity.ProductId,
                ProductName = entity.Product?.Name ?? "",
                TargetAmount = entity.TargetAmount,
                TargetUnits = entity.TargetUnits
            };
        }

        private async Task<ProductQuotaDto> Update(CreateOrEditProductQuotaDto input)
        {
            var entity = await _productQuotaRepository.GetAsync(input.Id);

            entity.PeriodYear = input.PeriodYear;
            entity.PeriodMonth = input.PeriodMonth;
            entity.HospitalId = input.HospitalId;
            entity.ProductCategoryId = input.ProductCategoryId;
            entity.ProductId = input.ProductId;
            entity.TargetAmount = input.TargetAmount;
            entity.TargetUnits = input.TargetUnits;

            await _productQuotaRepository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Load navigation properties for return DTO
            entity = await _productQuotaRepository
                .GetAll()
                .Include(pq => pq.Hospital)
                .Include(pq => pq.ProductCategory)
                .Include(pq => pq.Product)
                .FirstOrDefaultAsync(e => e.Id == entity.Id);

            return new ProductQuotaDto
            {
                Id = entity.Id,
                PeriodYear = entity.PeriodYear,
                PeriodMonth = entity.PeriodMonth,
                HospitalId = entity.HospitalId,
                HospitalName = entity.Hospital?.FacilityName ?? "",
                ProductCategoryId = entity.ProductCategoryId,
                ProductCategoryName = entity.ProductCategory?.Name ?? "",
                ProductId = entity.ProductId,
                ProductName = entity.Product?.Name ?? "",
                TargetAmount = entity.TargetAmount,
                TargetUnits = entity.TargetUnits
            };
        }

        public async Task Delete(EntityDto<int> input)
        {
            await _productQuotaRepository.DeleteAsync(input.Id);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private IQueryable<ProductQuota> CreateFilteredQuery(GetAllProductQuotasInput input)
        {
            var query = _productQuotaRepository.GetAll();

            // Year filter
            if (input.YearFilter.HasValue)
            {
                query = query.Where(pq => pq.PeriodYear == input.YearFilter.Value);
            }

            // Month filter
            if (input.MonthFilter.HasValue)
            {
                query = query.Where(pq => pq.PeriodMonth == input.MonthFilter.Value);
            }

            // Hospital filter
            if (input.HospitalIdFilter.HasValue)
            {
                query = query.Where(pq => pq.HospitalId == input.HospitalIdFilter.Value);
            }

            // Product Category filter
            if (input.ProductCategoryIdFilter.HasValue)
            {
                query = query.Where(pq => pq.ProductCategoryId == input.ProductCategoryIdFilter.Value);
            }

            // General text filter
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                query = query.Where(pq =>
                    pq.Hospital.FacilityName.Contains(input.Filter) ||
                    pq.ProductCategory.Name.Contains(input.Filter) ||
                    (pq.Product != null && pq.Product.Name.Contains(input.Filter))
                );
            }

            return query;
        }
    }
}

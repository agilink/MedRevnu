using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using ATI.Revenue.Application.Products.Dtos;
using ATI.Revenue.Domain.Entities;
using ATI.Revenue.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Products
{
    public class ProductsAppService : AsyncCrudAppService<Product, ProductDto, int, GetAllProductsInput, CreateOrEditProductDto>, IProductsAppService
    {
        private readonly IRepository<Product, int> _productRepository;

        public ProductsAppService(IRepository<Product, int> productRepository) : base(productRepository)
        {
            _productRepository = productRepository;
        }

        protected override async Task<Product> GetEntityByIdAsync(int id)
        {
            var entity = await _productRepository.GetAll()
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductSubcategory)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (entity == null)
                throw new EntityNotFoundException(typeof(Product), id);

            return entity;
        }

        public override async Task<PagedResultDto<ProductDto>> GetAllAsync(GetAllProductsInput input)
        {
            IQueryable<Product> query = _productRepository.GetAll()
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductSubcategory);

            if (input.CategoryIdFilter.HasValue)
                query = query.Where(p => p.ProductCategoryId == input.CategoryIdFilter.Value);

            if (input.SubcategoryIdFilter.HasValue)
                query = query.Where(p => p.SubproductCategoryId == input.SubcategoryIdFilter.Value);

            var totalCount = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.Name)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount > 0 ? input.MaxResultCount : 10)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProductCode = p.ProductCode ?? "",
                    Name = p.Name ?? "",
                    Manufacturer = p.Manufacturer ?? "",
                    ModelNo = p.ModelNo ?? "",
                    Description = p.Description ?? "",
                    ProductCategoryId = p.ProductCategoryId,
                    ProductCategoryName = p.ProductCategory != null ? p.ProductCategory.Name : null,
                    SubproductCategoryId = p.SubproductCategoryId,
                    SubproductCategoryName = p.ProductSubcategory != null ? p.ProductSubcategory.SubcategoryName : null,
                    BasePrice = p.BasePrice,
                    Cost = p.Cost,
                    Price = p.Price,
                    IsSystem = p.IsSystem,
                    IsActive = p.IsActive
                })
                .ToListAsync();

            return new PagedResultDto<ProductDto>(totalCount, products);
        }

        public async Task<ListResultDto<ProductDto>> GetAllActive()
        {
            var products = await _productRepository
                .GetAll()
                .Where(p => p.IsActive)
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductSubcategory)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProductCode = p.ProductCode ?? "",
                    Name = p.Name ?? "",
                    Manufacturer = p.Manufacturer ?? "",
                    ModelNo = p.ModelNo ?? "",
                    Description = p.Description ?? "",
                    ProductCategoryId = p.ProductCategoryId,
                    ProductCategoryName = p.ProductCategory != null ? (p.ProductCategory.Name ?? "") : "",
                    SubproductCategoryId = p.SubproductCategoryId,
                    SubproductCategoryName = p.ProductSubcategory != null ? (p.ProductSubcategory.SubcategoryName ?? "") : "",
                    ImplantType = p.ProductSubcategory != null ? p.ProductSubcategory.ImplantType : (ImplantType?)null,
                    BasePrice = p.BasePrice,
                    Cost = p.Cost,
                    Price = p.Price,
                    IsSystem = p.IsSystem,
                    IsActive = p.IsActive
                })
                .ToListAsync();

            return new ListResultDto<ProductDto>(products);
        }
    }
}

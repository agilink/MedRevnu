using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using ATI.Revenue.Application.Products.Dtos;
using ATI.Revenue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Products
{
    public class ProductsAppService : AsyncCrudAppService<Product, ProductDto, int, PagedAndSortedResultRequestDto, CreateOrEditProductDto>, IProductsAppService
    {
        private readonly IRepository<Product, int> _productRepository;

        public ProductsAppService(IRepository<Product, int> productRepository) : base(productRepository)
        {
            _productRepository = productRepository;
        }

        protected override IQueryable<Product> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            return _productRepository.GetAll()
                .Include(p => p.ProductCategory);
        }

        public async Task<ListResultDto<ProductDto>> GetAllActive()
        {
            var products = await _productRepository
                .GetAll()
                .Where(p => p.IsActive)
                .Include(p => p.ProductCategory)
                .ToListAsync();

            return new ListResultDto<ProductDto>(
                ObjectMapper.Map<List<ProductDto>>(products)
            );
        }
    }
}
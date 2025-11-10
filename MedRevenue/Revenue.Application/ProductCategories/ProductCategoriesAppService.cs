using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using ATI.Revenue.Application.ProductCategories.Dtos;
using ATI.Revenue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.ProductCategories
{
    public class ProductCategoriesAppService : ApplicationService
    {
        private readonly IRepository<ProductCategory, int> _productCategoryRepository;

        public ProductCategoriesAppService(IRepository<ProductCategory, int> productCategoryRepository)
        {
            _productCategoryRepository = productCategoryRepository;
        }

        public async Task<ListResultDto<ProductCategoryDto>> GetAll()
        {
            var categories = await _productCategoryRepository
                .GetAll()
                .Include(pc => pc.Products)
                .ToListAsync();

            var dtos = categories.Select(c => new ProductCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductCount = c.Products.Count
            }).ToList();

            return new ListResultDto<ProductCategoryDto>(dtos);
        }
    }
}
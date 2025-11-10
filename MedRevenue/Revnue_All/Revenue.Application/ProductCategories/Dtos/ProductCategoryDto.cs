using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ATI.Revenue.Domain.Entities;

namespace ATI.Revenue.Application.ProductCategories.Dtos
{
    [AutoMapFrom(typeof(ProductCategory))]
    public class ProductCategoryDto : EntityDto<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProductCount { get; set; }
    }
}
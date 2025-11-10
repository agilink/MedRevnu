using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ATI.Revenue.Domain.Entities;

namespace ATI.Revenue.Application.Products.Dtos
{
    [AutoMapFrom(typeof(Product))]
    public class ProductDto : EntityDto<int>
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string ModelNo { get; set; }
        public string Description { get; set; }
        public int? ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}
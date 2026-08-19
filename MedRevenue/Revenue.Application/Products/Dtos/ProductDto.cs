using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ATI.Revenue.Domain.Entities;
using ATI.Revenue.Domain.Enums;

namespace ATI.Revenue.Application.Products.Dtos
{
    [AutoMapFrom(typeof(Product))]
    public class ProductDto : EntityDto<int>
    {
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string ModelNo { get; set; }
        public string Description { get; set; }
        public int? ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public int? SubproductCategoryId { get; set; }
        public string SubproductCategoryName { get; set; }

        /// <summary>
        /// The implant type implied by the product's subcategory. De Novo and Gen
        /// Change are modelled as separate subcategories ("Single Chamber" vs
        /// "Single Chamber Gen Change"), so a product determines which it is.
        /// Null only where a product has no subcategory assigned.
        /// </summary>
        public ImplantType? ImplantType { get; set; }
        public decimal BasePrice { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}

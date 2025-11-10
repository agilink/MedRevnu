using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ATI.Revenue.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ATI.Revenue.Application.Products.Dtos
{
    [AutoMapTo(typeof(Product))]
    public class CreateOrEditProductDto : EntityDto<int?>
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Manufacturer { get; set; }

        [StringLength(100)]
        public string ModelNo { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public int? ProductCategoryId { get; set; }

        [Required]
        public decimal Cost { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsActive { get; set; }
    }
}
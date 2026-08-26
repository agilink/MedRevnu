using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ATI.Revenue.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ATI.Revenue.Application.Products.Dtos
{
    [AutoMapTo(typeof(Product))]
    public class CreateOrEditProductDto : EntityDto<int>
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(200)]
        public string? Manufacturer { get; set; }

        [StringLength(100)]
        public string? ModelNo { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int? ProductCategoryId { get; set; }

        /// <summary>
        /// The subcategory decides whether this device is De Novo or Gen Change, which the
        /// case form validates against. A product without one cannot be checked.
        /// </summary>
        public int? SubproductCategoryId { get; set; }

        [StringLength(30)]
        public string? ProductCode { get; set; }

        /// <summary>
        /// The price a case falls back to when the hospital has no contracted price for
        /// this device. This is the figure the entry form and Rate Chart actually use.
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal BasePrice { get; set; }

        /// <summary>True for a full system, false for a generator only.</summary>
        public bool IsSystem { get; set; }

        [Required]
        public decimal Cost { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsActive { get; set; }
    }
}
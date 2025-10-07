using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using ATI.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace ATI.MedRevnu.Application.LafayetteQuota.Dto
{
    public class ProductDto : EntityDto
    {
        public string Name { get; set; }
        public string ModelNo { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int? ManufacturerId { get; set; }
        public decimal? ListPrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUserName { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUserName { get; set; }
    }

    public class CreateOrEditProductDto : EntityDto<int?>
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(200)]
        public string ModelNo { get; set; }

        [StringLength(200)]
        public string Category { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public int? ManufacturerId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ListPrice { get; set; }

        public bool IsActive { get; set; }

        public CreateOrEditProductDto()
        {
            IsActive = true;
        }
    }

    public class GetProductForViewDto
    {
        public ProductDto Product { get; set; }
    }

    public class GetProductForEditOutput
    {
        public CreateOrEditProductDto Product { get; set; }
    }

    public class GetAllProductsInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public string? Filter { get; set; }
        public string? NameFilter { get; set; }
        public string? CategoryFilter { get; set; }
        public string? ModelNoFilter { get; set; }
        
        public bool? IsActiveFilter { get; set; }
        public decimal? ListPriceFromFilter { get; set; }
        public decimal? ListPriceToFilter { get; set; }
        
        // DataTables properties
        public int Draw { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "name asc";
            }
        }
    }

    public class GetAllProductsForLookupTableInput : PagedAndSortedInputDto
    {
        public string Filter { get; set; }
        
        // DataTables properties
        public int Draw { get; set; }
    }

    public class GetAllProductsForLookupTableOutput
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
    }
}
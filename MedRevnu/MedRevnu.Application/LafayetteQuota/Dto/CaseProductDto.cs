using Abp.Application.Services.Dto;
using ATI.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace ATI.MedRevnu.Application.LafayetteQuota.Dto
{
    public class CaseProductDto : EntityDto
    {
        public int CaseId { get; set; }
        public int ProductId { get; set; }
        public decimal Revenue { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
        public decimal? Cost { get; set; }
        public decimal Profit { get; set; }
        public decimal RevenuePerUnit { get; set; }
        public string ProductName { get; set; }
        public string ProductModelNo { get; set; }
        public string ProductCategory { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUserName { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUserName { get; set; }
    }

    public class CreateOrEditCaseProductDto : EntityDto<int?>
    {
        [Required]
        public int CaseId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Revenue { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Cost { get; set; }

        public CreateOrEditCaseProductDto()
        {
            Quantity = 1;
            Revenue = 0;
        }
    }

    public class GetCaseProductForViewDto
    {
        public CaseProductDto CaseProduct { get; set; }
        public string CaseNumber { get; set; }
        public string ProductName { get; set; }
    }

    public class GetCaseProductForEditOutput
    {
        public CreateOrEditCaseProductDto CaseProduct { get; set; }
        public string CaseNumber { get; set; }
        public string ProductName { get; set; }
    }

    public class GetAllCaseProductsInput : PagedAndSortedInputDto
    {
        public string? Filter { get; set; }
        public int? CaseIdFilter { get; set; }
        public int? ProductIdFilter { get; set; }
        public decimal? RevenueFromFilter { get; set; }
        public decimal? RevenueToFilter { get; set; }
        public int? QuantityFromFilter { get; set; }
        public int? QuantityToFilter { get; set; }
        public decimal? CostFromFilter { get; set; }
        public decimal? CostToFilter { get; set; }
    }

    public class GetAllCaseProductsForLookupTableInput : PagedAndSortedInputDto
    {
        public string? Filter { get; set; }
    }

    public class GetAllCaseProductsForLookupTableOutput
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
    }
}
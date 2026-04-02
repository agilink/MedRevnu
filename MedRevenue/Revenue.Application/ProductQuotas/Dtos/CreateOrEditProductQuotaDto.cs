using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ATI.Revenue.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ATI.Revenue.Application.ProductQuotas.Dtos
{
    [AutoMapTo(typeof(ProductQuota))]
    [AutoMapFrom(typeof(ProductQuota))]
    public class CreateOrEditProductQuotaDto : EntityDto<int>
    {
        [Required]
        [Range(2000, 2100, ErrorMessage = "Year must be between 2000 and 2100")]
        public int PeriodYear { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
        public int PeriodMonth { get; set; }

        [Required]
        public int HospitalId { get; set; }

        [Required]
        public int ProductCategoryId { get; set; }

        public int? ProductId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Target Amount must be greater than 0")]
        public decimal TargetAmount { get; set; }

        public int? TargetUnits { get; set; }
    }
}

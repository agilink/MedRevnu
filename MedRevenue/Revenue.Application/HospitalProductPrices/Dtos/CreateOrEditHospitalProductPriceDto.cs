using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace ATI.Revenue.Application.HospitalProductPrices.Dtos
{
    public class CreateOrEditHospitalProductPriceDto : EntityDto<int>
    {
        [Required]
        public int HospitalId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [MaxLength(100)]
        public string? ProductCode { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be greater than 0")]
        public decimal UnitPrice { get; set; }

        [Required]
        public DateTime? EffectiveDate { get; set; }

        public bool IsActive { get; set; }
    }
}

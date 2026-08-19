using Abp.Application.Services.Dto;
using System;

namespace ATI.Revenue.Application.HospitalProductPrices.Dtos
{
    public class HospitalProductPriceDto : EntityDto<int>
    {
        public int HospitalId { get; set; }
        public string HospitalName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool IsActive { get; set; }
    }
}

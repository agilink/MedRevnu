using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ATI.Revenue.Domain.Entities;

namespace ATI.Revenue.Application.ProductQuotas.Dtos
{
    [AutoMapFrom(typeof(ProductQuota))]
    public class ProductQuotaDto : EntityDto<int>
    {
        public int PeriodYear { get; set; }
        public int PeriodMonth { get; set; }
        public int HospitalId { get; set; }
        public string HospitalName { get; set; }
        public int ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal TargetAmount { get; set; }
        public int? TargetUnits { get; set; }
    }
}

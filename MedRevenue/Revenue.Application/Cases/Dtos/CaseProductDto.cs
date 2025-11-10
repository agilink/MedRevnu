using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ATI.Revenue.Domain.Entities;

namespace ATI.Revenue.Application.Cases.Dtos
{
    [AutoMapFrom(typeof(CaseProduct))]
    [AutoMapTo(typeof(CaseProduct))]
    public class CaseProductDto : EntityDto<int>
    {
        public int CaseId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ATI.Revenue.Domain.Entities;
using ATI.Revenue.Domain.Enums;
using System;

namespace ATI.Revenue.Application.ProcedureTransactions.Dtos
{
    [AutoMapFrom(typeof(ProcedureTransaction))]
    public class ProcedureTransactionDto : EntityDto<int>
    {
        public DateTime ProcedureDate { get; set; }
        public int? HospitalId { get; set; }
        public string HospitalName { get; set; }
        public int PhysicianId { get; set; }
        public string PhysicianName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public ImplantType ImplantType { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
}

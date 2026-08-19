using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ATI.Revenue.Domain.Entities;
using ATI.Revenue.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ATI.Revenue.Application.ProcedureTransactions.Dtos
{
    [AutoMapTo(typeof(ProcedureTransaction))]
    [AutoMapFrom(typeof(ProcedureTransaction))]
    public class CreateOrEditProcedureTransactionDto : EntityDto<int>
    {
        [Required]
        public DateTime ProcedureDate { get; set; }

        // HospitalId is auto-populated from Physician's FacilityId - not required in input
        public int? HospitalId { get; set; }

        [Required]
        public int PhysicianId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public ImplantType ImplantType { get; set; } = ImplantType.DeNovo;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be greater than 0")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Transaction Amount must be greater than 0")]
        public decimal TotalAmount { get; set; }
    }
}

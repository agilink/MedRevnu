using Abp.Application.Services.Dto;
using ATI.Revenue.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ATI.Revenue.Application.ProcedureQuotas.Dtos
{
    public class CreateOrEditProcedureQuotaDto : EntityDto<int>
    {
        [Required]
        public int ProcedureTypeId { get; set; }

        public int? FacilityId { get; set; }

        [Required]
        public QuotaPeriod QuotaPeriod { get; set; }

        [Required]
        public decimal QuotaValue { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}

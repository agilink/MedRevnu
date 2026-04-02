using Abp.Application.Services.Dto;
using ATI.Revenue.Domain.Enums;
using System;

namespace ATI.Revenue.Application.ProcedureQuotas.Dtos
{
    public class ProcedureQuotaDto : EntityDto<int>
    {
        public int ProcedureTypeId { get; set; }
        public string ProcedureTypeName { get; set; }
        public int? FacilityId { get; set; }
        public string FacilityName { get; set; }
        public QuotaPeriod QuotaPeriod { get; set; }
        public string QuotaPeriodName { get; set; }
        public decimal QuotaValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Notes { get; set; }
        public DateTime CreationTime { get; set; }
    }
}

using Abp.Application.Services.Dto;
using ATI.Revenue.Domain.Enums;
using System;

namespace ATI.Revenue.Application.ProcedureQuotas.Dtos
{
    public class GetAllProcedureQuotasInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public int? ProcedureTypeIdFilter { get; set; }
        public int? FacilityIdFilter { get; set; }
        public QuotaPeriod? QuotaPeriodFilter { get; set; }
        public DateTime? StartDateFilter { get; set; }
        public DateTime? EndDateFilter { get; set; }
    }
}

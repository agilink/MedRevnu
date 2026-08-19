using Abp.Application.Services.Dto;
using ATI.Revenue.Domain.Enums;
using System;

namespace ATI.Revenue.Application.ProcedureTransactions.Dtos
{
    public class GetAllProcedureTransactionsInput : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
        public int? YearFilter { get; set; }
        public int? MonthFilter { get; set; }
        public int? HospitalIdFilter { get; set; }
        public int? PhysicianIdFilter { get; set; }
        public int? ProductIdFilter { get; set; }
        public ImplantType? ImplantTypeFilter { get; set; }
        public DateTime? MinProcedureDateFilter { get; set; }
        public DateTime? MaxProcedureDateFilter { get; set; }
    }
}

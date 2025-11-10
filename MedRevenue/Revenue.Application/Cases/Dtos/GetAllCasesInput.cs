using Abp.Application.Services.Dto;
using System;

namespace ATI.Revenue.Application.Cases.Dtos
{
    public class GetAllCasesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public string CaseNumberFilter { get; set; }
        public string ClientNameFilter { get; set; }
        public string StatusFilter { get; set; }
        public DateTime? MinCaseDateFilter { get; set; }
        public DateTime? MaxCaseDateFilter { get; set; }
    }
}
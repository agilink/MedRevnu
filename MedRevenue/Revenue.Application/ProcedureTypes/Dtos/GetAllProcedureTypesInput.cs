using Abp.Application.Services.Dto;
using ATI.Revenue.Domain.Enums;

namespace ATI.Revenue.Application.ProcedureTypes.Dtos
{
    public class GetAllProcedureTypesInput : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
        public string? NameFilter { get; set; }
        public string? CodeFilter { get; set; }
        public CategoryGroup? CategoryGroupFilter { get; set; }
        public bool? IsActiveFilter { get; set; }
    }
}

using Abp.Application.Services.Dto;
using ATI.Admin.Domain.Enums;

namespace ATI.Revenue.Application.Physicians.Dtos
{
    public class GetAllPhysiciansInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public int? HospitalIdFilter { get; set; }
        public EmployeeStatus? EmployeeStatusFilter { get; set; }

        /// <summary>
        /// Finds physicians with no hospital assigned. Their cases would be recorded
        /// without a hospital, so they need to be findable.
        /// </summary>
        public bool UnassignedHospitalOnly { get; set; }
    }
}

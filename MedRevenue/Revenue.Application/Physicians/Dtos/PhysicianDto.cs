using Abp.Application.Services.Dto;
using ATI.Admin.Domain.Enums;

namespace ATI.Revenue.Application.Physicians.Dtos
{
    public class PhysicianDto : EntityDto<int>
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }

        public int? HospitalId { get; set; }
        public string HospitalName { get; set; }

        public string EmployeeId { get; set; }
        public string EmailWork { get; set; }
        public string MobileNumber { get; set; }

        public PersonnelType? PersonnelType { get; set; }
        public EmployeeStatus? EmployeeStatus { get; set; }
        public string Notes { get; set; }

        /// <summary>The login created for this physician, if any.</summary>
        public long? UserId { get; set; }

        public string UserName { get; set; }

        /// <summary>Drives the "is a user" flag in the grid.</summary>
        public bool HasUser => UserId.HasValue;
    }
}

using Abp.Application.Services.Dto;
using ATI.Admin.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ATI.Revenue.Application.Physicians.Dtos
{
    public class CreateOrEditPhysicianDto : EntityDto<int>
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        /// <summary>
        /// The hospital the physician operates at. Revenue transactions take their
        /// hospital from this, so a physician without one leaves transactions unassigned.
        /// </summary>
        public int? HospitalId { get; set; }

        [StringLength(50)]
        public string EmployeeId { get; set; }

        [StringLength(200)]
        [EmailAddress]
        public string EmailWork { get; set; }

        [StringLength(50)]
        public string MobileNumber { get; set; }

        public PersonnelType? PersonnelType { get; set; }

        public EmployeeStatus? EmployeeStatus { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }
    }
}

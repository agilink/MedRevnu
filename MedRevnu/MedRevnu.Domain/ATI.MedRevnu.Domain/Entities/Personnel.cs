using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ATI.MedRevnu.Domain.Entities
{
    /// <summary>
    /// Medical personnel entity for Lafayette Quota system (doctors, technicians, etc.)
    /// </summary>
    public class Personnel : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(200)]
        public string LastName { get; set; }

        [StringLength(200)]
        public string EmailAddress { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(200)]
        public string Specialty { get; set; }

        [StringLength(100)]
        public string LicenseNumber { get; set; }

        public bool IsActive { get; set; }

        // Foreign Keys
        public int? CompanyId { get; set; }
        public long? UserId { get; set; }

        // Navigation properties
        public virtual ICollection<Case> Cases { get; set; }

        public Personnel()
        {
            Cases = new HashSet<Case>();
            IsActive = true;
        }

        public string FullName => $"{FirstName} {LastName}";
    }
}
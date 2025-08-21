using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ATI.MedRevnu.Domain.Entities
{
    /// <summary>
    /// Medical device product entity for Lafayette Quota system
    /// </summary>
    public class Product : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(200)]
        public string ModelNo { get; set; }

        [StringLength(200)]
        public string Category { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public int? ManufacturerId { get; set; }

        public decimal? ListPrice { get; set; }

        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ICollection<CaseProduct> CaseProducts { get; set; }

        public Product()
        {
            CaseProducts = new HashSet<CaseProduct>();
            IsActive = true;
        }
    }
}
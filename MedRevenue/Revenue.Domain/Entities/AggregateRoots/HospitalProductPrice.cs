using Abp.Domain.Entities.Auditing;
using ATI.Admin.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATI.Revenue.Domain.Entities
{
    [Table("HospitalProductPrice", Schema = "REV")]
    public class HospitalProductPrice : AuditedAggregateRoot<int>
    {
        [Required]
        public int HospitalId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [MaxLength(100)]
        public string ProductCode { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("HospitalId")]
        public virtual Facility Hospital { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}

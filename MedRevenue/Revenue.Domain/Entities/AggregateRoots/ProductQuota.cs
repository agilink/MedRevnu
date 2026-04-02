using Abp.Domain.Entities.Auditing;
using ATI.Admin.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATI.Revenue.Domain.Entities
{
    [Table("ProductQuota", Schema = "REV")]
    public class ProductQuota : AuditedAggregateRoot<int>
    {
        [Required]
        public int HospitalId { get; set; }

        [Required]
        public int ProductCategoryId { get; set; }

        public int? ProductId { get; set; } // Optional - specific device

        [Required]
        public int PeriodMonth { get; set; } // 1-12

        [Required]
        public int PeriodYear { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TargetAmount { get; set; }

        public int? TargetUnits { get; set; }

        [ForeignKey("HospitalId")]
        public virtual Facility Hospital { get; set; }

        [ForeignKey("ProductCategoryId")]
        public virtual ProductCategory ProductCategory { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}

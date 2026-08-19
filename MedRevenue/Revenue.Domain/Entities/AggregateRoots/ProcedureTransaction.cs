using Abp.Domain.Entities.Auditing;
using ATI.Admin.Domain.Entities;
using ATI.Revenue.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATI.Revenue.Domain.Entities
{
    [Table("ProcedureTransaction", Schema = "REV")]
    public class ProcedureTransaction : FullAuditedAggregateRoot<int>
    {
        [Required]
        public DateTime ProcedureDate { get; set; }

        public int? HospitalId { get; set; }

        [Required]
        public int PhysicianId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public ImplantType ImplantType { get; set; }

        public int Quantity { get; set; } = 1;

        [Required]
        public decimal UnitPrice { get; set; }

        public decimal TotalAmount { get; set; } // quantity × unit_price

        [ForeignKey("HospitalId")]
        public virtual Facility Hospital { get; set; }

        [ForeignKey("PhysicianId")]
        public virtual Personnel Physician { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}

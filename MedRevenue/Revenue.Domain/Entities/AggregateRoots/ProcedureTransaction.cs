using Abp.Domain.Entities.Auditing;
using ATI.Admin.Domain.Entities;
using ATI.Revenue.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATI.Revenue.Domain.Entities
{
    /// <summary>
    /// One case: a procedure performed by a physician at a hospital on a date, using one
    /// or more devices.
    /// </summary>
    /// <remarks>
    /// A case is a single row here, identified by its case number, and the devices used
    /// hang off it as <see cref="ProcedureTransactionProduct"/> lines. This replaced a
    /// one-product-per-row shape that could not record a procedure using two or three
    /// devices, and whose Quantity column doubled as both a device count and a case
    /// count - so a multi-device procedure would have inflated every case-count report.
    /// </remarks>
    [Table("ProcedureTransaction", Schema = "REV")]
    public class ProcedureTransaction : FullAuditedAggregateRoot<int>
    {
        public ProcedureTransaction()
        {
            this.Products = new HashSet<ProcedureTransactionProduct>();
        }

        /// <summary>The business identifier for the case. Unique.</summary>
        [Required]
        [MaxLength(50)]
        public string CaseNumber { get; set; }

        [Required]
        public DateTime ProcedureDate { get; set; }

        public int? HospitalId { get; set; }

        [Required]
        public int PhysicianId { get; set; }

        /// <summary>
        /// De Novo or Gen Change for the case as a whole. Every device on the case must
        /// belong to a subcategory of this type.
        /// </summary>
        [Required]
        public ImplantType ImplantType { get; set; }

        /// <summary>Where the case has reached in the billing cycle.</summary>
        [Required]
        public CaseStatus Status { get; set; } = CaseStatus.Open;

        /// <summary>Free-text note about the case.</summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Sum of the line totals, unless deliberately overridden for a negotiated case
        /// price.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [ForeignKey("HospitalId")]
        public virtual Facility Hospital { get; set; }

        [ForeignKey("PhysicianId")]
        public virtual Personnel Physician { get; set; }

        public virtual ICollection<ProcedureTransactionProduct> Products { get; set; }
    }
}

using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATI.Revenue.Domain.Entities
{
    /// <summary>
    /// A device used in one case. A procedure can involve two or three devices, so the
    /// products live on lines rather than on the case itself.
    /// </summary>
    /// <remarks>
    /// Revenue broken down by product or product category must be read from these lines,
    /// not from the case, because a single case can span more than one category. Case
    /// counts, by contrast, come from the case rows - summing line quantities would
    /// report a three-device procedure as three cases.
    /// </remarks>
    [Table("ProcedureTransactionProduct", Schema = "REV")]
    public class ProcedureTransactionProduct : FullAuditedEntity<int>
    {
        [Required]
        public int ProcedureTransactionId { get; set; }

        [Required]
        public int ProductId { get; set; }

        /// <summary>How many of this device were used. Normally one.</summary>
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// Defaults to the hospital's contracted price for this product on the procedure
        /// date, and may be overridden for a negotiated price.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        /// <summary>Quantity x UnitPrice.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }

        [ForeignKey("ProcedureTransactionId")]
        public virtual ProcedureTransaction ProcedureTransaction { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}

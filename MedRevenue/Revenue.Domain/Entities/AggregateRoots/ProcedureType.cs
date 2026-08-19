using Abp.Domain.Entities.Auditing;
using ATI.Revenue.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATI.Revenue.Domain.Entities
{
    [Table("ProcedureType", Schema = "REV")]
    public class ProcedureType : AuditedAggregateRoot<int>
    {
        public ProcedureType()
        {
            this.Cases = new HashSet<Case>();
            this.ProcedureQuotas = new HashSet<ProcedureQuota>();
        }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        public CategoryGroup CategoryGroup { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public int DisplayOrder { get; set; }

        public virtual ICollection<Case> Cases { get; set; }
        public virtual ICollection<ProcedureQuota> ProcedureQuotas { get; set; }
    }
}

using Abp.Domain.Entities.Auditing;
using ATI.Revenue.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATI.Revenue.Domain.Entities
{
    [Table("ProcedureQuota", Schema = "REV")]
    public class ProcedureQuota : AuditedAggregateRoot<int>
    {
        public int ProcedureTypeId { get; set; }

        public int? FacilityId { get; set; }

        public QuotaPeriod QuotaPeriod { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal QuotaValue { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        [ForeignKey("ProcedureTypeId")]
        public virtual ProcedureType ProcedureType { get; set; }
    }
}

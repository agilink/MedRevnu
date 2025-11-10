using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATI.Revenue.Domain.Entities
{
    [Table("Case", Schema = "REV")]
    public class Case : AuditedAggregateRoot<int>
    {
        public Case()
        {
            this.CaseProducts = new HashSet<CaseProduct>();
        }

        public string CaseNumber { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public DateTime CaseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }

        public virtual ICollection<CaseProduct> CaseProducts { get; set; }
    }
}
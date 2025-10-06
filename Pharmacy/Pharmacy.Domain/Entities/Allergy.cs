using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATI.Pharmacy.Domain.Entities
{
    public partial class Allergy : AuditedEntity<int>
    {
        public string AllergyName { get; set; }
        public string Description { get; set; }
        public string Reaction { get; set; }
        public string Severity { get; set; }
        public Nullable<System.DateTime> NotedDate { get; set; }       
        public bool Active { get; set; } = true;
    }
}

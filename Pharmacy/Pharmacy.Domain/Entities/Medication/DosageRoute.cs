
namespace ATI.Pharmacy.Domain.Entities
{
    using Abp.Domain.Entities;
    using System.Collections.Generic;

    public partial class DosageRoute:Entity<int>, IMayHaveTenant
    {
        public DosageRoute()
        {
            this.Medication = new HashSet<Medication>();
        }
    
        public int? TenantId { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }

        public bool Active { get; set; } = true;
    
        public virtual ICollection<Medication> Medication { get; set; }
        
    }
}

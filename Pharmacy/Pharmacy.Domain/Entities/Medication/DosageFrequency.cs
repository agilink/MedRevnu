

namespace ATI.Pharmacy.Domain.Entities
{
    using Abp.Domain.Entities;
    using System.Collections.Generic;

    public partial class DosageFrequency : Entity<int>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public DosageFrequency()
        {
            this.Medication = new HashSet<Medication>();
        }
    
       
        public string Description { get; set; }
    
       public virtual ICollection<Medication> Medication { get; set; }
    }
}

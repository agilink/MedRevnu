namespace ATI.Admin.Domain.Entities
{
    using Abp.Domain.Entities;
    using System;
    using System.Collections.Generic;
    
    public partial class UomType : Entity<int>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public UomType()
        {
            this.Uom = new HashSet<Uom>();
        }

        public string Type { get; set; }
    
        public virtual ICollection<Uom> Uom { get; set; }
    }
}

namespace ATI.Admin.Domain.Entities
{
    using Abp.Domain.Entities;
    using System;
    using System.Collections.Generic;

    public partial class Uom: Entity<int>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Uom()
        {
           
        }

        public Nullable<int> UomTypeID { get; set; }
        public string UomName { get; set; }
        public string UomDescription { get; set; }
            
        public virtual UomType UomType { get; set; }
        
    }
}

namespace ATI.Pharmacy.Domain.Entities
{
    using Abp.Domain.Entities;
    using System;
    using System.Collections.Generic;
    
    public partial class DosageStrength : Entity<int>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Description { get; set; }
    }
}

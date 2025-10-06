using Abp.Domain.Entities;

namespace ATI.Pharmacy.Domain.Entities
{    
    public partial class DosageDuration:Entity<int>,IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Description { get; set; }
       
    }
}

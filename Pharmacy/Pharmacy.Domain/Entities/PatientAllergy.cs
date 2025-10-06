namespace ATI.Pharmacy.Domain.Entities
{
    using Abp.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class PatientAllergy : Entity<int>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public int PatientId { get; set; }
        public int? AllergyId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        [ForeignKey("AllergyId")]
        public virtual Allergy? Allergy { get; set; }

        public string? Other { get; set; }
    }
}

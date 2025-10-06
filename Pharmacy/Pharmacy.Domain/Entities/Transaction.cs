
namespace ATI.Pharmacy.Domain.Entities
{
    using Abp.Domain.Entities;
    using System;
    using System.Collections.Generic;
    
    public partial class Transaction : Entity<int>
    {
      
        public Nullable<int> PrescriptionId { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string PaymentMethod { get; set; }
    
        public virtual Prescription Prescription { get; set; }
    }
}

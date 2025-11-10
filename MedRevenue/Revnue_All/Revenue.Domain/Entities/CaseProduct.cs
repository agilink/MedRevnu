using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATI.Revenue.Domain.Entities
{
    [Table("CaseProduct", Schema = "REV")]
    public class CaseProduct : Entity<int>
    {
        public int CaseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }

        [ForeignKey("CaseId")]
        public virtual Case Case { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
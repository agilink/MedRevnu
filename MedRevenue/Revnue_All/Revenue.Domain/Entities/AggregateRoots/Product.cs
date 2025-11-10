using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATI.Revenue.Domain.Entities
{
    [Table("Product", Schema = "REV")]
    public class Product : AuditedAggregateRoot<int>
    {
        public Product()
        {
            this.CaseProducts = new HashSet<CaseProduct>();
        }

        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string ModelNo { get; set; }
        public string Description { get; set; }
        public int? ProductCategoryId { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("ProductCategoryId")]
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<CaseProduct> CaseProducts { get; set; }
    }
}
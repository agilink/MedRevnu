using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATI.Revenue.Domain.Entities
{
    [Table("ProductCategory", Schema = "REV")]
    public class ProductCategory : AuditedAggregateRoot<int>
    {
        public ProductCategory()
        {
            this.Products = new HashSet<Product>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
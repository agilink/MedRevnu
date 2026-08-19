using Abp.Domain.Entities.Auditing;
using ATI.Revenue.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATI.Revenue.Domain.Entities
{
    [Table("ProductSubcategory", Schema = "REV")]
    public class ProductSubcategory : AuditedAggregateRoot<int>
    {
        public ProductSubcategory()
        {
            this.Products = new HashSet<Product>();
        }

        public int ProductCategoryId { get; set; }
        public string SubcategoryName { get; set; }
        public ImplantType ImplantType { get; set; }
        public string? Description { get; set; }

        [ForeignKey("ProductCategoryId")]
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}

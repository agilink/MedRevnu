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

        public int? SubproductCategoryId { get; set; }
        public string? ProductCode { get; set; }
        public string Name { get; set; }
        public bool IsSystem { get; set; }
        public decimal BasePrice { get; set; }

        // Legacy fields - keeping for backwards compatibility
        public string? Manufacturer { get; set; }
        public string? ModelNo { get; set; }
        public string? Description { get; set; }
        public int? ProductCategoryId { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("ProductCategoryId")]
        public virtual ProductCategory ProductCategory { get; set; }

        [ForeignKey("SubproductCategoryId")]
        public virtual ProductSubcategory ProductSubcategory { get; set; }

        public virtual ICollection<CaseProduct> CaseProducts { get; set; }
    }
}
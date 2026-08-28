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
        /// <summary>
        /// Which kind of procedure this subcategory belongs on, or null for one that
        /// belongs on either.
        /// </summary>
        /// <remarks>
        /// Leads and accessories are fitted during both new implants and generator
        /// changes, so forcing them to pick one would make them unusable on half the
        /// cases. Null means "no restriction" and is treated that way by the device
        /// dropdown and by the save-time check.
        /// </remarks>
        public ImplantType? ImplantType { get; set; }
        public string? Description { get; set; }

        [ForeignKey("ProductCategoryId")]
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}

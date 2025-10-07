using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;

namespace ATI.MedRevnu.Domain.Entities
{
    /// <summary>
    /// Junction entity representing products used in a specific case
    /// Contains revenue and quantity information
    /// </summary>
    public class CaseProduct : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        public int CaseId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public decimal Revenue { get; set; }

        public int Quantity { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public decimal? Cost { get; set; }

        // Navigation properties
        public virtual Case Case { get; set; }
        public virtual Product Product { get; set; }

        public CaseProduct()
        {
            Quantity = 1;
            Revenue = 0;
        }

        // Domain methods
        public decimal GetProfit()
        {
            return Revenue - (Cost ?? 0);
        }

        public decimal GetRevenuePerUnit()
        {
            return Quantity > 0 ? Revenue / Quantity : 0;
        }
    }
}
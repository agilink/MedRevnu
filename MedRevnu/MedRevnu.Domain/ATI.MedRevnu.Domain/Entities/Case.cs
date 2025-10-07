using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ATI.MedRevnu.Domain.Entities
{
    /// <summary>
    /// Medical case entity representing a procedure or treatment case
    /// This is the aggregate root for Case management
    /// </summary>
    public class Case : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [StringLength(200)]
        public string CaseNumber { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(200)]
        public string ProcedureType { get; set; }

        [StringLength(100)]
        public string Status { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        // Foreign Keys
        public int? DoctorId { get; set; }
        public int? HospitalId { get; set; }
        public int? FacilityId { get; set; }

        // Navigation properties
        public virtual Personnel Doctor { get; set; }
        public virtual ICollection<CaseProduct> CaseProducts { get; set; }

        public Case()
        {
            CaseProducts = new HashSet<CaseProduct>();
            Date = DateTime.Now;
            Status = "Active";
        }

        // Domain methods
        public decimal GetTotalRevenue()
        {
            return CaseProducts?.Sum(cp => cp.Revenue) ?? 0;
        }

        public void AddProduct(int productId, decimal revenue, int quantity = 1)
        {
            var existingProduct = CaseProducts.FirstOrDefault(cp => cp.ProductId == productId);
            
            if (existingProduct != null)
            {
                existingProduct.Quantity += quantity;
                existingProduct.Revenue += revenue;
            }
            else
            {
                CaseProducts.Add(new CaseProduct
                {
                    CaseId = this.Id,
                    ProductId = productId,
                    Revenue = revenue,
                    Quantity = quantity
                });
            }
        }

        public void RemoveProduct(int productId)
        {
            var product = CaseProducts.FirstOrDefault(cp => cp.ProductId == productId);
            if (product != null)
            {
                CaseProducts.Remove(product);
            }
        }
    }
}
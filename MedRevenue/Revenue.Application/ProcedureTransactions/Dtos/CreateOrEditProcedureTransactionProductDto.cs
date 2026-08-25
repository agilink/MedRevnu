using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace ATI.Revenue.Application.ProcedureTransactions.Dtos
{
    public class CreateOrEditProcedureTransactionProductDto : EntityDto<int>
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// Left at zero to take the hospital's contracted price for this product on the
        /// procedure date; set to override it.
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }
}

using Abp.Application.Services.Dto;
using ATI.Revenue.Domain.Enums;

namespace ATI.Revenue.Application.ProcedureTransactions.Dtos
{
    /// <summary>One device used in a case.</summary>
    public class ProcedureTransactionProductDto : EntityDto<int>
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string SubcategoryName { get; set; }

        /// <summary>The implant type this product implies, for showing why a line was rejected.</summary>
        public ImplantType? ProductImplantType { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}

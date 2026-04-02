using ATI.Revenue.Application.ProcedureTransactions.Dtos;

namespace ATI.Revenue.Web.PageModel.ProcedureTransactions
{
    public class CreateOrEditProcedureTransactionModalViewModel
    {
        public CreateOrEditProcedureTransactionDto ProcedureTransaction { get; set; }
        public bool IsEditMode { get; set; }
    }
}

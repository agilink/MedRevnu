using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Revenue.Application.ProcedureTransactions.Dtos;
using System;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.ProcedureTransactions
{
    public interface IProcedureTransactionsAppService : IApplicationService
    {
        Task<PagedResultDto<ProcedureTransactionDto>> GetAll(GetAllProcedureTransactionsInput input);
        Task<GetProcedureTransactionForViewDto> GetProcedureTransactionForView(int id);
        Task<GetProcedureTransactionForEditOutput> GetProcedureTransactionForEdit(EntityDto<int> input);
        Task<ProcedureTransactionDto> CreateOrEdit(CreateOrEditProcedureTransactionDto input);
        Task Delete(EntityDto<int> input);
        Task<decimal> GetProductPriceByHospital(int hospitalId, int productId);
        Task<decimal> GetEffectiveUnitPrice(int? hospitalId, int productId, DateTime asOfDate);
    }
}

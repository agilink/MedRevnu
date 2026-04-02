using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Revenue.Application.ProcedureQuotas.Dtos;
using System;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.ProcedureQuotas
{
    public interface IProcedureQuotasAppService : IApplicationService
    {
        Task<PagedResultDto<ProcedureQuotaDto>> GetAll(GetAllProcedureQuotasInput input);
        Task<GetProcedureQuotaForViewDto> GetProcedureQuotaForView(int id);
        Task<GetProcedureQuotaForEditOutput> GetProcedureQuotaForEdit(EntityDto<int> input);
        Task CreateOrEdit(CreateOrEditProcedureQuotaDto input);
        Task Delete(EntityDto<int> input);
        Task<ProcedureQuotaDto> GetQuotaForProcedure(int procedureTypeId, DateTime date, int? facilityId = null);
    }
}

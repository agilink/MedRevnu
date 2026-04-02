using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Revenue.Application.ProcedureTypes.Dtos;
using ATI.Revenue.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.ProcedureTypes
{
    public interface IProcedureTypesAppService : IApplicationService
    {
        Task<PagedResultDto<ProcedureTypeDto>> GetAll(GetAllProcedureTypesInput input);
        Task<GetProcedureTypeForViewDto> GetProcedureTypeForView(int id);
        Task<GetProcedureTypeForEditOutput> GetProcedureTypeForEdit(EntityDto<int> input);
        Task CreateOrEdit(CreateOrEditProcedureTypeDto input);
        Task Delete(EntityDto<int> input);
        Task<List<ProcedureTypeDto>> GetAllActiveProcedureTypes();
        Task<List<ProcedureTypeDto>> GetProcedureTypesByCategory(CategoryGroup categoryGroup);
    }
}

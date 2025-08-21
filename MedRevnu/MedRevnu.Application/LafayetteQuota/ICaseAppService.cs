using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.MedRevnu.Application.LafayetteQuota.Dto;
using System.Threading.Tasks;

namespace ATI.MedRevnu.Application.LafayetteQuota
{
    public interface ICaseAppService : IApplicationService
    {
        Task<PagedResultDto<GetCaseForViewDto>> GetAll(GetAllCasesInput input);

        Task<GetCaseForViewDto> GetCaseForView(int id);

        Task<GetCaseForEditOutput> GetCaseForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditCaseDto input);

        Task Delete(EntityDto input);

        Task<PagedResultDto<GetAllCasesForLookupTableOutput>> GetAllCasesForLookupTable(
            GetAllCasesForLookupTableInput input);

        Task<PagedResultDto<GetAllPersonnelForLookupTableOutput>> GetAllPersonnelForLookupTable(
            GetAllPersonnelForLookupTableInput input);
    }
}
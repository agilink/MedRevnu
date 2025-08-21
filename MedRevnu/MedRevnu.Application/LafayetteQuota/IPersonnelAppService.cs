using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.MedRevnu.Application.LafayetteQuota.Dto;
using System.Threading.Tasks;

namespace ATI.MedRevnu.Application.LafayetteQuota
{
    public interface IPersonnelAppService : IApplicationService
    {
        Task<PagedResultDto<GetPersonnelForViewDto>> GetAll(GetAllPersonnelInput input);

        Task<GetPersonnelForViewDto> GetPersonnelForView(int id);

        Task<GetPersonnelForEditOutput> GetPersonnelForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditPersonnelDto input);

        Task Delete(EntityDto input);

        Task<PagedResultDto<GetAllPersonnelForLookupTableOutput>> GetAllPersonnelForLookupTable(
            GetAllPersonnelForLookupTableInput input);
    }
}
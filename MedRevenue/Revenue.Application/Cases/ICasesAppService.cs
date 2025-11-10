using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Revenue.Application.Cases.Dtos;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Cases
{
   
    public interface ICasesAppService : IAsyncCrudAppService<CaseDto, int, GetAllCasesInput, CreateOrEditCaseDto>
    {
        Task<GetCaseForViewDto> GetCaseForView(int id);
        Task<GetCaseForEditOutput> GetCaseForEdit(EntityDto<int> input);
        Task<PagedResultDto<CaseDto>> GetAllFiltered(GetAllCasesInput input);
    }
}
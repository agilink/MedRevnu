using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.MedRevnu.Application.LafayetteQuota.Dto;
using System.Threading.Tasks;

namespace ATI.MedRevnu.Application.LafayetteQuota
{
    public interface IProductAppService : IApplicationService
    {
        Task<PagedResultDto<GetProductForViewDto>> GetAll(GetAllProductsInput input);

        Task<GetProductForViewDto> GetProductForView(int id);

        Task<GetProductForEditOutput> GetProductForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditProductDto input);

        Task Delete(EntityDto input);

        Task<PagedResultDto<GetAllProductsForLookupTableOutput>> GetAllProductsForLookupTable(
            GetAllProductsForLookupTableInput input);
    }
}
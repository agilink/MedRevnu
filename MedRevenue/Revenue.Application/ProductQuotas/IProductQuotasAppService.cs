using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Revenue.Application.ProductQuotas.Dtos;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.ProductQuotas
{
    public interface IProductQuotasAppService : IApplicationService
    {
        Task<PagedResultDto<ProductQuotaDto>> GetAll(GetAllProductQuotasInput input);
        Task<GetProductQuotaForViewDto> GetProductQuotaForView(int id);
        Task<GetProductQuotaForEditOutput> GetProductQuotaForEdit(EntityDto<int> input);
        Task<ProductQuotaDto> CreateOrEdit(CreateOrEditProductQuotaDto input);
        Task Delete(EntityDto<int> input);
    }
}

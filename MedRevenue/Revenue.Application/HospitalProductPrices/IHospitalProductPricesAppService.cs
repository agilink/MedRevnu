using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Revenue.Application.HospitalProductPrices.Dtos;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.HospitalProductPrices
{
    public interface IHospitalProductPricesAppService : IApplicationService
    {
        Task<PagedResultDto<HospitalProductPriceDto>> GetAll(GetAllHospitalProductPricesInput input);
        Task<GetHospitalProductPriceForViewDto> GetHospitalProductPriceForView(int id);
        Task<GetHospitalProductPriceForEditOutput> GetHospitalProductPriceForEdit(EntityDto<int> input);
        Task<HospitalProductPriceDto> CreateOrEdit(CreateOrEditHospitalProductPriceDto input);
        Task Delete(EntityDto<int> input);
    }
}

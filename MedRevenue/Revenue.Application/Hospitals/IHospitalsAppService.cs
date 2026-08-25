using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Revenue.Application.Hospitals.Dtos;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Hospitals
{
    public interface IHospitalsAppService : IApplicationService
    {
        Task<PagedResultDto<HospitalDto>> GetAll(GetAllHospitalsInput input);
        Task<GetHospitalForViewDto> GetHospitalForView(int id);
        Task<GetHospitalForEditOutput> GetHospitalForEdit(EntityDto<int> input);
        Task<HospitalDto> CreateOrEdit(CreateOrEditHospitalDto input);
        Task Delete(EntityDto<int> input);
    }
}

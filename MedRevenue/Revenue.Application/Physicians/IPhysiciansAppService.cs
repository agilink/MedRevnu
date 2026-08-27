using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Revenue.Application.Physicians.Dtos;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Physicians
{
    public interface IPhysiciansAppService : IApplicationService
    {
        Task<PagedResultDto<PhysicianDto>> GetAll(GetAllPhysiciansInput input);
        Task<GetPhysicianForViewDto> GetPhysicianForView(int id);
        Task<GetPhysicianForEditOutput> GetPhysicianForEdit(EntityDto<int> input);
        Task<PhysicianDto> CreateOrEdit(CreateOrEditPhysicianDto input);
        Task Delete(EntityDto<int> input);
        Task<CreatePhysicianUserOutput> CreateUserForPhysician(EntityDto<int> input);
    }
}

using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Authorization.Users.Dto;
using ATI.Pharmacy.Dtos;
using GetUsersInput = ATI.Pharmacy.Dtos.GetUsersInput;

namespace ATI.Pharmacy.Application
{
    public  interface IUserExtendedAppService : IApplicationService
    {
        Task<PagedResultDto<EmployeeListDto>> GetUsers(GetUsersInput input);
        Task<CreateOrEditUserInputDto> GetEmployeeForEdit(EntityDto entityDto);
    }
}

using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Authorization.Permissions.Dto;

namespace ATI.Authorization.Permissions
{
    public interface IPermissionAppService : IApplicationService
    {
        ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions();
    }
}

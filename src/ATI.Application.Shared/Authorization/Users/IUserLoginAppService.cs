using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Authorization.Users.Dto;

namespace ATI.Authorization.Users
{
    public interface IUserLoginAppService : IApplicationService
    {
        Task<PagedResultDto<UserLoginAttemptDto>> GetUserLoginAttempts(GetLoginAttemptsInput input);
        Task<string> GetExternalLoginProviderNameByUser(long userId);
    }
}

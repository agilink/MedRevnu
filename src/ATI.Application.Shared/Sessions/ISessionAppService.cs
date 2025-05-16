using System.Threading.Tasks;
using Abp.Application.Services;
using ATI.Sessions.Dto;

namespace ATI.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();

        Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken();
    }
}

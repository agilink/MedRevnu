using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Caching.Dto;

namespace ATI.Caching
{
    public interface ICachingAppService : IApplicationService
    {
        ListResultDto<CacheDto> GetAllCaches();

        Task ClearCache(EntityDto<string> input);

        Task ClearAllCaches();
        
        bool CanClearAllCaches();
    }
}

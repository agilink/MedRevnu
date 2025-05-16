using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Common.Dto;
using ATI.Editions.Dto;

namespace ATI.Common
{
    public interface ICommonLookupAppService : IApplicationService
    {
        Task<ListResultDto<SubscribableEditionComboboxItemDto>> GetEditionsForCombobox(bool onlyFreeItems = false);

        Task<PagedResultDto<FindUsersOutputDto>> FindUsers(FindUsersInput input);

        GetDefaultEditionNameOutput GetDefaultEditionName();
    }
}
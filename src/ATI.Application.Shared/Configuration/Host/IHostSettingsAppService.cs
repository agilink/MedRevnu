using System.Threading.Tasks;
using Abp.Application.Services;
using ATI.Configuration.Host.Dto;

namespace ATI.Configuration.Host
{
    public interface IHostSettingsAppService : IApplicationService
    {
        Task<HostSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(HostSettingsEditDto input);

        Task SendTestEmail(SendTestEmailInput input);
    }
}

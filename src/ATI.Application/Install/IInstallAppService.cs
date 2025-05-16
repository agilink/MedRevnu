using System.Threading.Tasks;
using Abp.Application.Services;
using ATI.Install.Dto;

namespace ATI.Install
{
    public interface IInstallAppService : IApplicationService
    {
        Task Setup(InstallDto input);

        AppSettingsJsonDto GetAppSettingsJson();

        CheckDatabaseOutput CheckDatabase();
    }
}
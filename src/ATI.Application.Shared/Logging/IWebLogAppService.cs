using Abp.Application.Services;
using ATI.Dto;
using ATI.Logging.Dto;

namespace ATI.Logging
{
    public interface IWebLogAppService : IApplicationService
    {
        GetLatestWebLogsOutput GetLatestWebLogs();

        FileDto DownloadWebLogs();
    }
}

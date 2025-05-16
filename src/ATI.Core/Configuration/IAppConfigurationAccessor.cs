using Microsoft.Extensions.Configuration;

namespace ATI.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}

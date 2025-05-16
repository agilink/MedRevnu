using System.Threading.Tasks;
using ATI.Authorization.Users;

namespace ATI.WebHooks
{
    public interface IAppWebhookPublisher
    {
        Task PublishTestWebhook();
    }
}

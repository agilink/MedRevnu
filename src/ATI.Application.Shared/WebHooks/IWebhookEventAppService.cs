using System.Threading.Tasks;
using Abp.Webhooks;

namespace ATI.WebHooks
{
    public interface IWebhookEventAppService
    {
        Task<WebhookEvent> Get(string id);
    }
}

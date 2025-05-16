using Abp.Application.Services.Dto;
using Abp.Webhooks;
using ATI.WebHooks.Dto;

namespace ATI.Web.Areas.Core.Models.Webhooks
{
    public class CreateOrEditWebhookSubscriptionViewModel
    {
        public WebhookSubscription WebhookSubscription { get; set; }

        public ListResultDto<GetAllAvailableWebhooksOutput> AvailableWebhookEvents { get; set; }
    }
}

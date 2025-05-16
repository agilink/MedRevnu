using System.Threading.Tasks;
using Abp.Application.Services;
using ATI.MultiTenancy.Payments.Dto;
using ATI.MultiTenancy.Payments.Stripe.Dto;

namespace ATI.MultiTenancy.Payments.Stripe
{
    public interface IStripePaymentAppService : IApplicationService
    {
        Task ConfirmPayment(StripeConfirmPaymentInput input);

        StripeConfigurationDto GetConfiguration();
        
        Task<string> CreatePaymentSession(StripeCreatePaymentSessionInput input);
    }
}
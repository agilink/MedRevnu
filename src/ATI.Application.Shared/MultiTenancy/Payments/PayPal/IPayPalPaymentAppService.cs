using System.Threading.Tasks;
using Abp.Application.Services;
using ATI.MultiTenancy.Payments.PayPal.Dto;

namespace ATI.MultiTenancy.Payments.PayPal
{
    public interface IPayPalPaymentAppService : IApplicationService
    {
        Task ConfirmPayment(long paymentId, string paypalOrderId);

        PayPalConfigurationDto GetConfiguration();
    }
}

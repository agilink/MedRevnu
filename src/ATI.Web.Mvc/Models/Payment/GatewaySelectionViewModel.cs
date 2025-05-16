using System.Collections.Generic;
using System.Linq;
using ATI.MultiTenancy.Payments;
using ATI.MultiTenancy.Payments.Dto;

namespace ATI.Web.Models.Payment
{
    public class GatewaySelectionViewModel
    {
        public SubscriptionPaymentDto Payment { get; set; }
        
        public List<PaymentGatewayModel> PaymentGateways { get; set; }

        public bool AllowRecurringPaymentOption()
        {
            return Payment.AllowRecurringPayment() && PaymentGateways.Any(gateway => gateway.SupportsRecurringPayments);
        }
    }
}

using ATI.Editions;
using ATI.Editions.Dto;
using ATI.MultiTenancy.Payments;
using ATI.Security;
using ATI.MultiTenancy.Payments.Dto;

namespace ATI.Web.Models.TenantRegistration
{
    public class TenantRegisterViewModel
    {
        public int? EditionId { get; set; }

        public EditionSelectDto Edition { get; set; }
        
        public PasswordComplexitySetting PasswordComplexitySetting { get; set; }

        public EditionPaymentType EditionPaymentType { get; set; }
        
        public SubscriptionStartType? SubscriptionStartType { get; set; }
        
        public PaymentPeriodType? PaymentPeriodType { get; set; }
        
        public string SuccessUrl { get; set; }
        
        public string ErrorUrl { get; set; }
    }
}

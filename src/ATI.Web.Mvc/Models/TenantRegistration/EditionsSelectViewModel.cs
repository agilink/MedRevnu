using System.Collections.Generic;
using System.Linq;
using Abp.AutoMapper;
using ATI.MultiTenancy.Dto;
using ATI.MultiTenancy.Payments;

namespace ATI.Web.Models.TenantRegistration
{
    [AutoMapFrom(typeof(EditionsSelectOutput))]
    public class EditionsSelectViewModel : EditionsSelectOutput
    {
        public List<PaymentPeriodType> GetAvailablePaymentPeriodTypes()
        {
            var result = new List<PaymentPeriodType>();
            
            if (EditionsWithFeatures.Any(e=> e.Edition.MonthlyPrice.HasValue))
            {
                result.Add(PaymentPeriodType.Monthly);
            }
            
            if (EditionsWithFeatures.Any(e=> e.Edition.AnnualPrice.HasValue))
            {
                result.Add(PaymentPeriodType.Annual);
            }
            
            return result;
        } 
    }
}

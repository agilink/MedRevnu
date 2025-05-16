using ATI.MultiTenancy.Dto;
using ATI.Sessions.Dto;

namespace ATI.Web.Areas.Core.Models.Editions
{
    public class SubscriptionDashboardViewModel
    {
        public GetCurrentLoginInformationsOutput LoginInformations { get; set; }
        
        public EditionsSelectOutput Editions { get; set; }
    }
}

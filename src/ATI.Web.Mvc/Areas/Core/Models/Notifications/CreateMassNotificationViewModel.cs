using System.Collections.Generic;
using Abp.Notifications;

namespace ATI.Web.Areas.Core.Models.Notifications
{
    public class CreateMassNotificationViewModel
    {
        public List<string> TargetNotifiers { get; set; }
    
        public NotificationSeverity Severity { get; set; }
    }
}
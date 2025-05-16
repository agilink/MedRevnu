using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ATI.Web.Areas.Core.Models.Layout;
using ATI.Web.Views;

namespace ATI.Web.Areas.Core.Views.Shared.Components.CoreRecentNotifications
{
    public class CoreRecentNotificationsViewComponent : ATIViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(string cssClass, string iconClass = "flaticon-alert-2 unread-notification fs-2")
        {
            var model = new RecentNotificationsViewModel
            {
                CssClass = cssClass,
                IconClass = iconClass
            };
            
            return Task.FromResult<IViewComponentResult>(View(model));
        }
    }
}

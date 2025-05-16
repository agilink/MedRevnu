using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ATI.Web.Areas.Core.Models.Layout;
using ATI.Web.Views;

namespace ATI.Web.Areas.Core.Views.Shared.Components.CoreChatToggler
{
    public class CoreChatTogglerViewComponent : ATIViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(string cssClass, string iconClass = "flaticon-chat-2 fs-4")
        {
            return Task.FromResult<IViewComponentResult>(View(new ChatTogglerViewModel
            {
                CssClass = cssClass,
                IconClass = iconClass
            }));
        }
    }
}

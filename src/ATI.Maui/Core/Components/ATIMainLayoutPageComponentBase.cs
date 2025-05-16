using JetBrains.Annotations;
using ATI.Maui.Services.Navigation;
using ATI.Maui.Services.Permission;
using ATI.Maui.Services.UI;

namespace ATI.Maui.Core.Components
{
    public class ATIMainLayoutPageComponentBase : ATIComponentBase
    {
        protected PageHeaderService PageHeaderService { get; set; }

        protected DomManipulatorService DomManipulatorService { get; set; }
        
        protected INavigationService NavigationService { get; set; }

        protected IPermissionService PermissionService { get; set; }

        public ATIMainLayoutPageComponentBase()
        {
            PageHeaderService = Resolve<PageHeaderService>();
            DomManipulatorService = Resolve<DomManipulatorService>();
            NavigationService = Resolve<INavigationService>();
            PermissionService = Resolve<IPermissionService>();
        }

        protected async Task SetPageHeader(string title)
        {
            PageHeaderService.Title = title;
            PageHeaderService.ClearButton();
            await DomManipulatorService.ClearModalBackdrop(JS);
        }
        
        protected async Task SetPageHeader(string title, string subTitle)
        {
            PageHeaderService.Title = title;
            PageHeaderService.SubTitle = subTitle;
            PageHeaderService.ClearButton();
            await DomManipulatorService.ClearModalBackdrop(JS);
        }
        
        protected async Task SetPageHeader(string title, string subTitle, List<PageHeaderButton> buttons)
        {
            PageHeaderService.Title = title;
            PageHeaderService.SubTitle = subTitle;
            PageHeaderService.SetButtons(buttons);
            await DomManipulatorService.ClearModalBackdrop(JS);
        }
    }
}

using Flurl.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ATI.ApiClient;
using ATI.Maui.Core;
using ATI.Maui.Core.Helpers;
using ATI.Maui.Core.Localization;
using ATI.Maui.Services.Account;
using ATI.Maui.Services.Navigation;
using ATI.Maui.Services.Tenants;
using ATI.Maui.Services.UI;
using Plugin.Connectivity;

#if ANDROID
using ATI.Maui.Core.Platforms.Android.HttpClient;
#endif
#if IOS
using ATI.Maui.Core.Platforms.iOS.HttpClient;
#endif

namespace ATI.Maui.Pages.Layout
{
    public partial class MainLayout
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IJSRuntime JS { get; set; }

        protected UserDialogsService UserDialogsService { get; set; }

        private bool IsConfigurationsInitialized { get; set; }

        private string _logoURL;

        protected override async Task OnInitializedAsync()
        {
            UserDialogsService = DependencyResolver.Resolve<UserDialogsService>();
            UserDialogsService.Initialize(JS);

            await UserDialogsService.Block();

            await CheckInternetAndStartApplication();

            var navigationService = DependencyResolver.Resolve<INavigationService>();
            navigationService.Initialize(NavigationManager);

            _logoURL = await DependencyResolver.Resolve<TenantCustomizationService>().GetTenantLogo();

            await SetLayout();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Task.Delay(200);
                await JS.InvokeVoidAsync("KTMenu.init");
            }
        }

        private async Task CheckInternetAndStartApplication()
        {
            if (CrossConnectivity.Current.IsConnected || ApiUrlConfig.IsLocal)
            {
                await StartApplication();
            }
            else
            {
                var isTryAgain = await UserDialogsService.Instance.Confirm(L.Localize("NoInternet"));
                if (!isTryAgain)
                {
                    CurrentApplicationCloser.Quit();
                }

                await CheckInternetAndStartApplication();
            }
        }

        private async Task StartApplication()
        {
            /*
              If you are using Genymotion Emulator, set DebugServerIpAddresses.Current = "10.0.3.2".
              If you are using a real Android device, set it as your computer's local IP and 
                 make sure your Android device and your computer is connecting to the internet via your local Wi-Fi.
           */
            DebugServerIpAddresses.Current = "10.0.2.2";

            ConfigureFlurlHttp();
            App.LoadPersistedSession();

            if (UserConfigurationManager.HasConfiguration)
            {
                IsConfigurationsInitialized = true;
                await UserDialogsService.UnBlock();

            }
            else
            {
                await UserConfigurationManager.GetAsync(async () =>
                {
                    IsConfigurationsInitialized = true;
                    await UserDialogsService.UnBlock();
                });
            }
        }

        private static void ConfigureFlurlHttp()
        {
            FlurlHttp.Clients.WithDefaults(builer => builer
            .OnError(call => HandleHttpErrorAsync(call))
            .ConfigureInnerHandler(hch =>
            {
#if ANDROID
                new AndroidHttpClientHandlerConfigurer().Configure(hch);
#elif IOS
                new IOSHttpClientHandlerConfigurer().Configure(hch);
#endif
            }));
        }

        private static async Task HandleHttpErrorAsync(FlurlCall call)
        {
            await new FlurlHttpErrorHandler().Handle(call);
        }

        private async Task SetLayout()
        {
            var dom = DependencyResolver.Resolve<DomManipulatorService>();
            await dom.ClearAllAttributes(JS, "body");
            await dom.SetAttribute(JS, "body", "id", "kt_app_body");
            await dom.SetAttribute(JS, "body", "data-kt-app-layout", "light-sidebar");
            await dom.SetAttribute(JS, "body", "data-kt-app-sidebar-enabled", "true");
            await dom.SetAttribute(JS, "body", "data-kt-app-sidebar-fixed", "true");
            await dom.SetAttribute(JS, "body", "data-kt-app-toolbar-enabled", "true");
            await dom.SetAttribute(JS, "body", "class", "app-default");
        }
    }
}

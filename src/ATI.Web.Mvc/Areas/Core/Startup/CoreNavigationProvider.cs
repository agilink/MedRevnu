using Abp.Application.Navigation;
using Abp.Authorization;
using Abp.Localization;
using ATI.Authorization;

namespace ATI.Web.Areas.Core.Startup
{
    public class CoreNavigationProvider : NavigationProvider
    {
        public const string MenuName = "App";

        public override void SetNavigation(INavigationProviderContext context)
        {
            var menu = context.Manager.Menus[MenuName] = new MenuDefinition(MenuName, new FixedLocalizableString("Main Menu"));

            menu
                .AddItem(new MenuItemDefinition(
                        CorePageNames.Host.Dashboard,
                        L("Dashboard"),
                        url: "Core/HostDashboard",
                        icon: "flaticon-line-graph",
                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Host_Dashboard)
                    )
                ).AddItem(new MenuItemDefinition(
                        CorePageNames.Common.Saas,
                        L("Saas"),
                        icon: "flaticon-users"
                    ).AddItem(new MenuItemDefinition(
                            CorePageNames.Host.Tenants,
                            L("Tenants"),
                            url: "Core/Tenants",
                            icon: "flaticon-list-3",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Tenants)
                        )
                    ).AddItem(new MenuItemDefinition(
                            CorePageNames.Host.Editions,
                            L("Editions"),
                            url: "Core/Editions",
                            icon: "flaticon-app",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Editions)
                        )
                    )
                )
                // Points at the Revenue dashboard, and is gated on that page's own
                // permission so the menu never offers a link that 403s.
                .AddItem(new MenuItemDefinition(
                        CorePageNames.Tenant.Dashboard,
                        L("Dashboard"),
                        url: "Revenue/Dashboard",
                        icon: "flaticon-line-graph",
                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Revenue_Dashboard)
                    )
                ).AddItem(new MenuItemDefinition(
                        CorePageNames.Common.Administration,
                        L("Administration"),
                        icon: "flaticon-interface-8"
                    ).AddItem(new MenuItemDefinition(
                            CorePageNames.Common.OrganizationUnits,
                            L("OrganizationUnits"),
                            url: "Core/OrganizationUnits",
                            icon: "flaticon-map",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_OrganizationUnits)
                        )
                    ).AddItem(new MenuItemDefinition(
                            CorePageNames.Common.Roles,
                            L("Roles"),
                            url: "Core/Roles",
                            icon: "flaticon-suitcase",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_Roles)
                        )
                    ).AddItem(new MenuItemDefinition(
                            CorePageNames.Common.Users,
                            L("Users"),
                            url: "Core/Users",
                            icon: "flaticon-users",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_Users)
                        )
                    ).AddItem(new MenuItemDefinition(
                            CorePageNames.Common.Languages,
                            L("Languages"),
                            url: "Core/Languages",
                            icon: "flaticon-tabs",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_Languages)
                        )
                    ).AddItem(new MenuItemDefinition(
                            CorePageNames.Common.AuditLogs,
                            L("AuditLogs"),
                            url: "Core/AuditLogs",
                            icon: "flaticon-folder-1",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_AuditLogs)
                        )
                    ).AddItem(new MenuItemDefinition(
                            CorePageNames.Host.Maintenance,
                            L("Maintenance"),
                            url: "Core/Maintenance",
                            icon: "flaticon-lock",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_Host_Maintenance)
                        )
                    ).AddItem(new MenuItemDefinition(
                            CorePageNames.Tenant.SubscriptionManagement,
                            L("Subscription"),
                            url: "Core/SubscriptionManagement",
                            icon: "flaticon-refresh",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_Tenant_SubscriptionManagement)
                        )
                    ).AddItem(new MenuItemDefinition(
                            CorePageNames.Common.UiCustomization,
                            L("VisualSettings"),
                            url: "Core/UiCustomization",
                            icon: "flaticon-medical",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_UiCustomization)
                        )
                    ).AddItem(new MenuItemDefinition(
                            CorePageNames.Common.WebhookSubscriptions,
                            L("WebhookSubscriptions"),
                            url: "Core/WebhookSubscription",
                            icon: "flaticon2-world",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_WebhookSubscription)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            CorePageNames.Common.DynamicProperties,
                            L("DynamicProperties"),
                            url: "Core/DynamicProperty",
                            icon: "flaticon-interface-8",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_DynamicProperties)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            CorePageNames.Common.Notifications,
                            L("Notifications"),
                            icon: "flaticon-alarm"
                        ).AddItem(new MenuItemDefinition(
                                CorePageNames.Common.Notifications_Inbox,
                                L("Inbox"),
                                url: "Core/Notifications",
                                icon: "flaticon-mail-1"
                            )
                        )
                        .AddItem(new MenuItemDefinition(
                                CorePageNames.Common.Notifications_MassNotifications,
                                L("MassNotifications"),
                                url: "Core/Notifications/MassNotifications",
                                icon: "flaticon-paper-plane",
                                permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_MassNotification)
                            )
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            CorePageNames.Host.Settings,
                            L("Settings"),
                            url: "Core/HostSettings",
                            icon: "flaticon-settings",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_Host_Settings)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            CorePageNames.Tenant.Settings,
                            L("Settings"),
                            url: "Core/Settings",
                            icon: "flaticon-settings",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_Tenant_Settings)
                        )
                    )
                // Revenue Transaction is the day-to-day screen, so it sits at the top
                // level on its own rather than behind a Revenue group.
                ).AddItem(new MenuItemDefinition(
                        CorePageNames.MedRevnuPages.RevenueTransactions,
                        L("RevenueTransactions"),
                        url: "Revenue/ProcedureTransactions",
                        icon: "flaticon-line-graph",
                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Revenue_ProcedureTransactions)
                    )
                ).AddItem(new MenuItemDefinition(
                        CorePageNames.MedRevnuPages.Reports,
                        L("Reports"),
                        icon: "flaticon-list-3",
                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Revenue_Reports)
                    )
                    .AddItem(new MenuItemDefinition(
                            "Reports.RateChart",
                            L("RateChart"),
                            url: "Revenue/Reports/RateChart",
                            icon: "flaticon-price-tag",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Revenue_Reports)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            "Reports.MonthlyRevenue",
                            L("MonthlyRevenue"),
                            url: "Revenue/Reports/MonthlyRevenue",
                            icon: "flaticon-calendar-3",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Revenue_Reports)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            "Reports.CasesByPerson",
                            L("CasesByPerson"),
                            url: "Revenue/Reports/CasesByPerson",
                            icon: "flaticon-user",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Revenue_Reports)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            "Reports.QuarterlyRollup",
                            L("QuarterlyRollup"),
                            url: "Revenue/Reports/QuarterlyRollup",
                            icon: "flaticon-calendar-2",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Revenue_Reports)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            "Reports.TransactionAmount",
                            L("TransactionAmount"),
                            url: "Revenue/Reports/TransactionAmount",
                            icon: "flaticon-coins",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Revenue_Reports)
                        )
                    )
                // Reference data - the things set up once and then left alone. Gated on
                // the Revenue root permission so it disappears for a role that has only
                // the dashboard and the transaction screen.
                ).AddItem(new MenuItemDefinition(
                        CorePageNames.MedRevnuPages.Configuration,
                        L("Configuration"),
                        icon: "flaticon-cogwheel-2",
                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Revenue)
                    )
                    .AddItem(new MenuItemDefinition(
                            "Configuration.Physicians",
                            L("Physicians"),
                            url: "Revenue/Physicians",
                            icon: "flaticon-user",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Revenue_Physicians)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            "Configuration.Hospitals",
                            L("Hospitals"),
                            url: "Revenue/Hospitals",
                            icon: "flaticon-home-2",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Revenue_Hospitals)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            "Configuration.Products",
                            L("Products"),
                            url: "Revenue/Products",
                            icon: "flaticon-medical",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Revenue_Products)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            "Configuration.ProductQuotas",
                            L("ProductQuotas"),
                            url: "Revenue/ProductQuotas",
                            icon: "flaticon-stopwatch",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Revenue_ProductQuotas)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            "Configuration.HospitalProductPrices",
                            L("HospitalProductPrices"),
                            url: "Revenue/HospitalProductPrices",
                            icon: "flaticon-price-tag",
                            permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Revenue_HospitalProductPrices)
                        )
                    )
                );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, ATIConsts.LocalizationSourceName);
        }
    }
}

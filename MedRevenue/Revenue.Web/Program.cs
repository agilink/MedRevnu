using Abp.AspNetCore;
using ATI.Revenue.Application;
using ATI.Revenue.Domain;
using ATI.Revenue.EntityFrameworkCore;
using ATI.Revenue.Web;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

// Standalone entry point — used only when running Revenue.Web independently.
// When referenced by ATI.Web.Mvc, this class is compiled as a library and not invoked.
public static class RevenueWebProgram
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews()
            .AddRazorRuntimeCompilation()
            .AddRazorOptions(options =>
            {
                options.ViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
                options.ViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
                options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });

        builder.Services.Configure<RazorViewEngineOptions>(options =>
        {
            options.AreaViewLocationFormats.Clear();
            options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
            options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
            options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
        });

        builder.Services.AddScoped<RevenueCoreModule>();
        builder.Services.AddScoped<RevenueApplicationModule>();
        builder.Services.AddScoped<RevenueEntityFrameworkCoreModule>();

        var connectionString = builder.Configuration.GetConnectionString("Default");
        builder.Services.AddDbContext<RevenueModuleDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        else
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseEmbeddedFiles();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "revenueArea",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        await app.RunAsync();
    }
}

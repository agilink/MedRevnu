using ATI.Revenue.Application;
using ATI.Revenue.Domain;
using ATI.Revenue.EntityFrameworkCore;
using ATI.Revenue.Web;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
    });

// Configure Areas
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.AreaViewLocationFormats.Clear();
    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
});

// Add Revenue Module services
builder.Services.AddScoped<RevenueCoreModule>();
builder.Services.AddScoped<RevenueApplicationModule>();
builder.Services.AddScoped<RevenueEntityFrameworkCoreModule>();

// Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<RevenueModuleDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "revenueArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

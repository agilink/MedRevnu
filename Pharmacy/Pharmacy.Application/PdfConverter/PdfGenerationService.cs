
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.ViewEngines;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using NReco.PdfGenerator;
using ATI.Configuration;
namespace ATI.Pharmacy;

public class PdfGenerationService : IPdfGenerationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICompositeViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IAppConfigurationAccessor _appConfigurationAccessor;
    public PdfGenerationService(

        IHttpContextAccessor httpContextAccessor,
        ICompositeViewEngine compositeViewEngine,
        ITempDataProvider tempDataProvider,
        IAppConfigurationAccessor appConfigurationAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _viewEngine = compositeViewEngine;
        _tempDataProvider = tempDataProvider;
        _appConfigurationAccessor = appConfigurationAccessor;
    }

    public async Task<byte[]> GeneratePdfFromPartialViewAsync(string viewPath, string viewName, object model)
    {
        var htmlContent = await RenderPartialViewToStringAsync(viewPath, viewName, model);

        var bytes = ConvertHtmlToPdf(htmlContent);

        return bytes;
    }

    private async Task<string> RenderPartialViewToStringAsync(string viewPath, string viewName, object model)
    {
        var nullActionDescriptor = new ControllerActionDescriptor
        {
            RouteValues = new Dictionary<string, string>(),
            Parameters = new List<ParameterDescriptor>(),
            DisplayName = string.Empty
        };
        var controllerContext = new ControllerContext
        {
            HttpContext = _httpContextAccessor.HttpContext,
            RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
            ActionDescriptor = nullActionDescriptor
        };

        var viewResult = _viewEngine.GetView(viewPath, viewName, false);

        if (viewResult.Success == false)
            throw new FileNotFoundException($"View {viewPath} not found.");

        using (var sw = new StringWriter())
        {
            var viewContext = new ViewContext(
                controllerContext,
                viewResult.View,
                new ViewDataDictionary<object>(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
                new TempDataDictionary(controllerContext.HttpContext, _tempDataProvider),
                sw,
                new HtmlHelperOptions()
            );
            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }
    }

    private byte[] ConvertHtmlToPdf(string htmlContent)
    {
        var licenceOwner = _appConfigurationAccessor.Configuration["NeoPdf:LicenceOwner"];
        var LicenceKey = _appConfigurationAccessor.Configuration["NeoPdf:LicenceKey"];
        // Initialize the PDF converter
        var htmlToPdf = new HtmlToPdfConverter();
        htmlToPdf.License.SetLicenseKey(licenceOwner, LicenceKey);
        htmlToPdf.CustomWkHtmlArgs = "--enable-local-file-access";

        byte[] pdfBytes = htmlToPdf.GeneratePdf(htmlContent);

        return pdfBytes;
    }
}
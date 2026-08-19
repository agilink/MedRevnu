using Microsoft.AspNetCore.Mvc;
using ATI.Revenue.Application.ProductCategories;
using ATI.Web.Controllers;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using ATI.Authorization;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    [AbpMvcAuthorize(AppPermissions.Pages_Revenue_Products)]
    public class ProductCategoriesController : ATIControllerBase
    {
        private readonly ProductCategoriesAppService _productCategoriesAppService;

        public ProductCategoriesController(ProductCategoriesAppService productCategoriesAppService)
        {
            _productCategoriesAppService = productCategoriesAppService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _productCategoriesAppService.GetAll();
            return View(categories.Items);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productCategoriesAppService.GetAll();
            return Json(result);
        }
    }
}
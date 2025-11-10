using Microsoft.AspNetCore.Mvc;
using ATI.Revenue.Application.ProductCategories;
using System.Threading.Tasks;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    public class ProductCategoriesController : Controller
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
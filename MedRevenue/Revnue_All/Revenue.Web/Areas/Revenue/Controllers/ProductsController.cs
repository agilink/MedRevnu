using Microsoft.AspNetCore.Mvc;
using ATI.Revenue.Application.Products;
using ATI.Revenue.Application.Products.Dtos;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    public class ProductsController : Controller
    {
        private readonly IProductsAppService _productsAppService;

        public ProductsController(IProductsAppService productsAppService)
        {
            _productsAppService = productsAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrEditProductDto input)
        {
            if (ModelState.IsValid)
            {
                await _productsAppService.CreateAsync(input);
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productsAppService.GetAsync(new EntityDto<int> { Id = id });
            var editDto = new CreateOrEditProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Manufacturer = product.Manufacturer,
                ModelNo = product.ModelNo,
                Description = product.Description,
                ProductCategoryId = product.ProductCategoryId,
                Cost = product.Cost,
                Price = product.Price,
                IsActive = product.IsActive
            };
            return View(editDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateOrEditProductDto input)
        {
            if (ModelState.IsValid)
            {
                await _productsAppService.UpdateAsync(input);
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productsAppService.GetAsync(new EntityDto<int> { Id = id });
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _productsAppService.DeleteAsync(new EntityDto<int> { Id = id });
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(PagedAndSortedResultRequestDto input)
        {
            var result = await _productsAppService.GetAllAsync(input);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActive()
        {
            var result = await _productsAppService.GetAllActive();
            return Json(result);
        }
    }
}
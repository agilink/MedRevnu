using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ATI.Revenue.Application.Products;
using ATI.Revenue.Application.Products.Dtos;
using ATI.Revenue.Domain.Entities;
using ATI.Web.Controllers;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    public class ProductsController : ATIControllerBase
    {
        private readonly IProductsAppService _productsAppService;
        private readonly IRepository<ProductCategory, int> _categoryRepository;
        private readonly IRepository<ProductSubcategory, int> _subcategoryRepository;

        public ProductsController(
            IProductsAppService productsAppService,
            IRepository<ProductCategory, int> categoryRepository,
            IRepository<ProductSubcategory, int> subcategoryRepository)
        {
            _productsAppService = productsAppService;
            _categoryRepository = categoryRepository;
            _subcategoryRepository = subcategoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await _categoryRepository.GetAll()
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSubcategoriesByCategory(int categoryId)
        {
            var subcategories = await _subcategoryRepository.GetAll()
                .Where(s => s.ProductCategoryId == categoryId)
                .OrderBy(s => s.SubcategoryName)
                .Select(s => new { id = s.Id, name = s.SubcategoryName })
                .ToListAsync();

            return Json(subcategories);
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> CreateOrEditModal(int? id = null)
        {
            CreateOrEditProductDto model;
            if (id.HasValue && id.Value > 0)
            {
                var product = await _productsAppService.GetAsync(new EntityDto<int> { Id = id.Value });
                model = new CreateOrEditProductDto
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
            }
            else
            {
                model = new CreateOrEditProductDto { IsActive = true };
            }
            return PartialView("_CreateOrEditModal", model);
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

        public async Task<IActionResult> DetailsModal(int id)
        {
            var product = await _productsAppService.GetAsync(new EntityDto<int> { Id = id });
            return PartialView("_ViewModal", product);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _productsAppService.DeleteAsync(new EntityDto<int> { Id = id });
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(GetAllProductsInput input)
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

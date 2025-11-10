using Abp.AspNetCore.Mvc.Authorization;
using Abp.Application.Services.Dto;
using Abp.ObjectMapping;
using ATI.Web.Controllers;
using ATI.Revenue.Application.Products;
using ATI.Revenue.Application.Products.Dtos;
using ATI.Revenue.Web.PageModel.Products;
using Microsoft.AspNetCore.Mvc;

namespace ATI.Revenue.Web.Controllers
{
    [Area("Revenue")]
    [AbpMvcAuthorize("Pages.Products")]
    public class ProductsController : ATIControllerBase
    {
        private readonly IProductsAppService _productsAppService;
        private readonly IObjectMapper _objectMapper;

        public ProductsController(
            IProductsAppService productsAppService,
            IObjectMapper objectMapper)
        {
            _productsAppService = productsAppService;
            _objectMapper = objectMapper;
        }

        public IActionResult Index()
        {
            var viewModel = new ProductsViewModel();
            return View(viewModel);
        }

        [AbpMvcAuthorize("Pages.Products.Create", "Pages.Products.Edit")]
        public async Task<PartialViewResult> CreateOrEditModal(int? id)
        {
            var viewModel = new CreateOrEditProductModalViewModel();
            
            if (id.HasValue)
            {
                var product = await _productsAppService.GetAsync(new EntityDto<int> { Id = id.Value });
                viewModel.Product = _objectMapper.Map<CreateOrEditProductDto>(product);
            }
            else
            {
                viewModel.Product = new CreateOrEditProductDto
                {
                    IsActive = true
                };
            }

            viewModel.IsEditMode = id.HasValue;
            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewProductModal(int id)
        {
            var product = await _productsAppService.GetAsync(new EntityDto<int> { Id = id });
            var viewModel = new ViewProductModalViewModel
            {
                Product = product
            };
            return PartialView("_ViewProductModal", viewModel);
        }
    }
}
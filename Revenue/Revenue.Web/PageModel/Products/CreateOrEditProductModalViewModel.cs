using ATI.Revenue.Application.Products.Dtos;

namespace ATI.Revenue.Web.PageModel.Products
{
    public class CreateOrEditProductModalViewModel
    {
        public CreateOrEditProductDto Product { get; set; }
        public bool IsEditMode { get; set; }
    }
}
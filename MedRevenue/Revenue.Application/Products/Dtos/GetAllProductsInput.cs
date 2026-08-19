using Abp.Application.Services.Dto;

namespace ATI.Revenue.Application.Products.Dtos
{
    public class GetAllProductsInput : PagedAndSortedResultRequestDto
    {
        public int? CategoryIdFilter { get; set; }
        public int? SubcategoryIdFilter { get; set; }
    }
}

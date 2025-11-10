using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Revenue.Application.Products.Dtos;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Products
{
    public interface IProductsAppService :  IAsyncCrudAppService<ProductDto, int, PagedAndSortedResultRequestDto, CreateOrEditProductDto>
    {
        Task<ListResultDto<ProductDto>> GetAllActive();
    }
}
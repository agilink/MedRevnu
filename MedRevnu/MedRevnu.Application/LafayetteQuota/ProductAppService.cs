using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using ATI.Authorization;
using ATI.Authorization.Users;
using ATI.MedRevnu.Application.LafayetteQuota.Dto;
using ATI.MedRevnu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ATI.MedRevnu.Application.LafayetteQuota
{
    [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Products)]
    public class ProductAppService : ApplicationService, IProductAppService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<User, long> _userRepository;

        public ProductAppService(
            IRepository<Product> productRepository,
            IRepository<User, long> userRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<PagedResultDto<GetProductForViewDto>> GetAll(GetAllProductsInput input)
        {
            // Debug logging to see what we actually receive
            Logger.Info($"GetAll called with input: Filter='{input.Filter}', NameFilter='{input.NameFilter}', CategoryFilter='{input.CategoryFilter}', ModelNoFilter='{input.ModelNoFilter}'");
            
            var filteredProducts = _productRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Name.Contains(input.Filter) || 
                                                                        e.Description.Contains(input.Filter) || 
                                                                        e.ModelNo.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name.Contains(input.NameFilter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.CategoryFilter), e => e.Category.Contains(input.CategoryFilter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.ModelNoFilter), e => e.ModelNo.Contains(input.ModelNoFilter))
                .WhereIf(input.IsActiveFilter.HasValue, e => e.IsActive == input.IsActiveFilter)
                .WhereIf(input.ListPriceFromFilter.HasValue, e => e.ListPrice >= input.ListPriceFromFilter)
                .WhereIf(input.ListPriceToFilter.HasValue, e => e.ListPrice <= input.ListPriceToFilter);

            var pagedAndFilteredProducts = filteredProducts
                .OrderBy(input.Sorting ?? "name asc")
                .PageBy(input);

            var products = from product in pagedAndFilteredProducts
                          join creatorUser in _userRepository.GetAll() on product.CreatorUserId equals creatorUser.Id into creatorJoin
                          from creator in creatorJoin.DefaultIfEmpty()
                          join modifierUser in _userRepository.GetAll() on product.LastModifierUserId equals modifierUser.Id into modifierJoin
                          from modifier in modifierJoin.DefaultIfEmpty()
                          select new GetProductForViewDto
                          {
                              Product = new ProductDto
                              {
                                  Id = product.Id,
                                  Name = product.Name,
                                  ModelNo = product.ModelNo,
                                  Category = product.Category,
                                  Description = product.Description,
                                  ManufacturerId = product.ManufacturerId,
                                  ListPrice = product.ListPrice,
                                  IsActive = product.IsActive,
                                  CreationTime = product.CreationTime,
                                  CreatorUserName = creator != null ? creator.UserName : "",
                                  LastModificationTime = product.LastModificationTime,
                                  LastModifierUserName = modifier != null ? modifier.UserName : ""
                              }
                          };

            var totalCount = await filteredProducts.CountAsync();

            return new PagedResultDto<GetProductForViewDto>(
                totalCount,
                await products.ToListAsync()
            );
        }

        public async Task<GetProductForViewDto> GetProductForView(int id)
        {
            var query = from product in _productRepository.GetAll()
                       join creatorUser in _userRepository.GetAll() on product.CreatorUserId equals creatorUser.Id into creatorJoin
                       from creator in creatorJoin.DefaultIfEmpty()
                       join modifierUser in _userRepository.GetAll() on product.LastModifierUserId equals modifierUser.Id into modifierJoin
                       from modifier in modifierJoin.DefaultIfEmpty()
                       where product.Id == id
                       select new GetProductForViewDto
                       {
                           Product = new ProductDto
                           {
                               Id = product.Id,
                               Name = product.Name,
                               ModelNo = product.ModelNo,
                               Category = product.Category,
                               Description = product.Description,
                               ManufacturerId = product.ManufacturerId,
                               ListPrice = product.ListPrice,
                               IsActive = product.IsActive,
                               CreationTime = product.CreationTime,
                               CreatorUserName = creator != null ? creator.UserName : "",
                               LastModificationTime = product.LastModificationTime,
                               LastModifierUserName = modifier != null ? modifier.UserName : ""
                           }
                       };

            return await query.FirstOrDefaultAsync() ?? throw new ArgumentException("Product not found");
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Products_Create)]
        public async Task<GetProductForEditOutput> GetProductForEdit(EntityDto input)
        {
            var product = await _productRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetProductForEditOutput
            {
                Product = ObjectMapper.Map<CreateOrEditProductDto>(product)
            };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Products_Create, AppPermissions.Pages_Administration_LafayetteQuota_Products_Edit)]
        public async Task CreateOrEdit(CreateOrEditProductDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Products_Create)]
        protected virtual async Task Create(CreateOrEditProductDto input)
        {
            var product = ObjectMapper.Map<Product>(input);

            await _productRepository.InsertAsync(product);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Products_Edit)]
        protected virtual async Task Update(CreateOrEditProductDto input)
        {
            var product = await _productRepository.GetAsync(input.Id.Value);
            ObjectMapper.Map(input, product);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_LafayetteQuota_Products_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _productRepository.DeleteAsync(input.Id);
        }

        public async Task<PagedResultDto<GetAllProductsForLookupTableOutput>> GetAllProductsForLookupTable(
            GetAllProductsForLookupTableInput input)
        {
            var query = _productRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Name.Contains(input.Filter))
                .Where(e => e.IsActive);

            var totalCount = await query.CountAsync();

            var productList = await query
                .OrderBy(input.Sorting ?? "name asc")
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = productList.Select(product => new GetAllProductsForLookupTableOutput
            {
                Id = product.Id.ToString(),
                DisplayName = product.Name + (!string.IsNullOrWhiteSpace(product.ModelNo) ? " - " + product.ModelNo : "")
            }).ToList();

            return new PagedResultDto<GetAllProductsForLookupTableOutput>(
                totalCount,
                lookupTableDtoList
            );
        }
    }
}
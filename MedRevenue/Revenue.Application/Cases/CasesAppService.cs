using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using ATI.Revenue.Application.Cases.Dtos;
using ATI.Revenue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Cases
{
    public class CasesAppService : AsyncCrudAppService<Case, CaseDto, int, GetAllCasesInput, CreateOrEditCaseDto>, ICasesAppService
    {
        private readonly IRepository<Case, int> _caseRepository;
        private readonly IRepository<CaseProduct, int> _caseProductRepository;
        private readonly IRepository<Product, int> _productRepository;

        public CasesAppService(
            IRepository<Case, int> caseRepository,
            IRepository<CaseProduct, int> caseProductRepository,
            IRepository<Product, int> productRepository
            ) : base(caseRepository)
        {
            _caseRepository = caseRepository;
            _caseProductRepository = caseProductRepository;
            _productRepository = productRepository;
        }

        public async Task<PagedResultDto<CaseDto>> GetAllFiltered(GetAllCasesInput input)
        {
            var query = CreateFilteredQuery(input)
                .Include(c => c.CaseProducts)
                    .ThenInclude(cp => cp.Product)
                .AsQueryable(); // Ensure the query remains IQueryable<Case>

            var totalCount = await query.CountAsync();

            query = ApplySorting(query, input); // Ensure ApplySorting returns IQueryable<Case>
            query = ApplyPaging(query, input);

            var entities = await query.ToListAsync();
            var dtos = ObjectMapper.Map<List<CaseDto>>(entities);

            return new PagedResultDto<CaseDto>(totalCount, dtos);
        }

        public async Task<GetCaseForViewDto> GetCaseForView(int id)
        {
            var entity = await _caseRepository
                .GetAll()
                .Include(c => c.CaseProducts)
                    .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entity == null)
            {
                throw new UserFriendlyException("Case not found");
            }

            var dto = ObjectMapper.Map<CaseDto>(entity);
            return new GetCaseForViewDto { Case = dto };
        }

        public async Task<GetCaseForEditOutput> GetCaseForEdit(EntityDto<int> input)
        {
            var entity = await _caseRepository
                .GetAll()
                .Include(c => c.CaseProducts)
                    .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(e => e.Id == input.Id);

            var editDto = ObjectMapper.Map<CreateOrEditCaseDto>(entity);
            
            return new GetCaseForEditOutput
            {
                Case = editDto
            };
        }

        protected override IQueryable<Case> CreateFilteredQuery(GetAllCasesInput input)
        {
            return _caseRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => e.CaseNumber.Contains(input.Filter) ||
                         e.ClientName.Contains(input.Filter) ||
                         e.Description.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.CaseNumberFilter),
                    e => e.CaseNumber.Contains(input.CaseNumberFilter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.ClientNameFilter),
                    e => e.ClientName.Contains(input.ClientNameFilter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.StatusFilter),
                    e => e.Status == input.StatusFilter)
                .WhereIf(input.MinCaseDateFilter != null,
                    e => e.CaseDate >= input.MinCaseDateFilter.Value)
                .WhereIf(input.MaxCaseDateFilter != null,
                    e => e.CaseDate <= input.MaxCaseDateFilter.Value);
        }

        public override async Task<CaseDto> CreateAsync(CreateOrEditCaseDto input)
        {
            var entity = ObjectMapper.Map<Case>(input);
            
            entity.Id = await _caseRepository.InsertAndGetIdAsync(entity);

            // Handle case products
            if (input.CaseProducts != null && input.CaseProducts.Any())
            {
                foreach (var productDto in input.CaseProducts)
                {
                    var caseProduct = new CaseProduct
                    {
                        CaseId = entity.Id,
                        ProductId = productDto.ProductId,
                        Quantity = productDto.Quantity,
                        UnitPrice = productDto.UnitPrice,
                        Discount = productDto.Discount,
                        TotalPrice = productDto.TotalPrice
                    };
                    await _caseProductRepository.InsertAsync(caseProduct);
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return MapToEntityDto(entity);
        }

        public override async Task<CaseDto> UpdateAsync(CreateOrEditCaseDto input)
        {
            var entity = await _caseRepository.GetAsync(input.Id);
            
            ObjectMapper.Map(input, entity);

            // Update case products
            await _caseProductRepository.DeleteAsync(cp => cp.CaseId == entity.Id);
            
            if (input.CaseProducts != null && input.CaseProducts.Any())
            {
                foreach (var productDto in input.CaseProducts)
                {
                    var caseProduct = new CaseProduct
                    {
                        CaseId = entity.Id,
                        ProductId = productDto.ProductId,
                        Quantity = productDto.Quantity,
                        UnitPrice = productDto.UnitPrice,
                        Discount = productDto.Discount,
                        TotalPrice = productDto.TotalPrice
                    };
                    await _caseProductRepository.InsertAsync(caseProduct);
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return MapToEntityDto(entity);
        }

        public async Task RemoveCaseProduct(EntityDto<int> input)
        {
            await _caseProductRepository.DeleteAsync(input.Id);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<CaseProductDto> AddOrUpdateCaseProduct(CaseProductDto input)
        {
            CaseProduct caseProduct;

            if (input.Id > 0)
            {
                // Update existing case product
                caseProduct = await _caseProductRepository.GetAsync(input.Id);
                caseProduct.ProductId = input.ProductId;
                caseProduct.Quantity = input.Quantity;
                caseProduct.UnitPrice = input.UnitPrice;
                caseProduct.Discount = input.Discount;
                caseProduct.TotalPrice = input.TotalPrice;

                await _caseProductRepository.UpdateAsync(caseProduct);
            }
            else
            {
                // Add new case product
                caseProduct = new CaseProduct
                {
                    CaseId = input.CaseId,
                    ProductId = input.ProductId,
                    Quantity = input.Quantity,
                    UnitPrice = input.UnitPrice,
                    Discount = input.Discount,
                    TotalPrice = input.TotalPrice
                };

                caseProduct.Id = await _caseProductRepository.InsertAndGetIdAsync(caseProduct);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<CaseProductDto>(caseProduct);
        }
    }
}
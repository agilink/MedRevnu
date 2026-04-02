using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using ATI.Admin.Domain.Entities;
using ATI.Revenue.Application.ProcedureTransactions.Dtos;
using ATI.Revenue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.ProcedureTransactions
{
    public class ProcedureTransactionsAppService : ApplicationService, IProcedureTransactionsAppService
    {
        private readonly IRepository<ProcedureTransaction, int> _procedureTransactionRepository;
        private readonly IRepository<Personnel, int> _personnelRepository;
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<Facility, int> _facilityRepository;
        private readonly IRepository<HospitalProductPrice, int> _hospitalProductPriceRepository;

        public ProcedureTransactionsAppService(
            IRepository<ProcedureTransaction, int> procedureTransactionRepository,
            IRepository<Personnel, int> personnelRepository,
            IRepository<Product, int> productRepository,
            IRepository<Facility, int> facilityRepository,
            IRepository<HospitalProductPrice, int> hospitalProductPriceRepository)
        {
            _procedureTransactionRepository = procedureTransactionRepository;
            _personnelRepository = personnelRepository;
            _productRepository = productRepository;
            _facilityRepository = facilityRepository;
            _hospitalProductPriceRepository = hospitalProductPriceRepository;
        }

        public async Task<PagedResultDto<ProcedureTransactionDto>> GetAll(GetAllProcedureTransactionsInput input)
        {
            var query = CreateFilteredQuery(input);

            var totalCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);
            else
                query = query.OrderByDescending(pt => pt.ProcedureDate);

            query = query.PageBy(input);

            var dtos = await query.Select(pt => new ProcedureTransactionDto
            {
                Id = pt.Id,
                ProcedureDate = pt.ProcedureDate,
                HospitalId = pt.HospitalId,
                HospitalName = pt.Hospital != null ? (pt.Hospital.FacilityName ?? "") : "",
                PhysicianId = pt.PhysicianId,
                PhysicianName = pt.Physician != null ? ((pt.Physician.FIRST_NAME ?? "") + " " + (pt.Physician.LAST_NAME ?? "")).Trim() : "",
                ProductId = pt.ProductId,
                ProductName = pt.Product != null ? (pt.Product.Name ?? "") : "",
                ProductCode = pt.Product != null ? (pt.Product.ProductCode ?? "") : "",
                ProcedureType = pt.ProcedureType,
                Quantity = pt.Quantity,
                UnitPrice = pt.UnitPrice,
                TotalAmount = pt.TotalAmount
            }).ToListAsync();

            return new PagedResultDto<ProcedureTransactionDto>(totalCount, dtos);
        }

        public async Task<GetProcedureTransactionForViewDto> GetProcedureTransactionForView(int id)
        {
            var dto = await _procedureTransactionRepository.GetAll()
                .Where(pt => pt.Id == id)
                .Select(pt => new ProcedureTransactionDto
                {
                    Id = pt.Id,
                    ProcedureDate = pt.ProcedureDate,
                    HospitalId = pt.HospitalId,
                    HospitalName = pt.Hospital != null ? (pt.Hospital.FacilityName ?? "") : "",
                    PhysicianId = pt.PhysicianId,
                    PhysicianName = pt.Physician != null ? ((pt.Physician.FIRST_NAME ?? "") + " " + (pt.Physician.LAST_NAME ?? "")).Trim() : "",
                    ProductId = pt.ProductId,
                    ProductName = pt.Product != null ? (pt.Product.Name ?? "") : "",
                    ProductCode = pt.Product != null ? (pt.Product.ProductCode ?? "") : "",
                    ProcedureType = pt.ProcedureType,
                    Quantity = pt.Quantity,
                    UnitPrice = pt.UnitPrice,
                    TotalAmount = pt.TotalAmount
                })
                .FirstOrDefaultAsync();

            if (dto == null)
                throw new UserFriendlyException("Procedure Transaction not found");

            return new GetProcedureTransactionForViewDto { ProcedureTransaction = dto };
        }

        public async Task<GetProcedureTransactionForEditOutput> GetProcedureTransactionForEdit(EntityDto<int> input)
        {
            var entity = await _procedureTransactionRepository
                .GetAll()
                .FirstOrDefaultAsync(e => e.Id == input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException("Procedure Transaction not found");
            }

            var editDto = new CreateOrEditProcedureTransactionDto
            {
                Id = entity.Id,
                ProcedureDate = entity.ProcedureDate,
                HospitalId = entity.HospitalId,
                PhysicianId = entity.PhysicianId,
                ProductId = entity.ProductId,
                ProcedureType = entity.ProcedureType,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                TotalAmount = entity.TotalAmount
            };

            return new GetProcedureTransactionForEditOutput
            {
                ProcedureTransaction = editDto
            };
        }

        public async Task<ProcedureTransactionDto> CreateOrEdit(CreateOrEditProcedureTransactionDto input)
        {
            if (input.Id == 0)
            {
                return await Create(input);
            }
            else
            {
                return await Update(input);
            }
        }

        private async Task<ProcedureTransactionDto> Create(CreateOrEditProcedureTransactionDto input)
        {
            // Auto-populate HospitalId from Physician's FacilityId
            var physician = await _personnelRepository.GetAsync(input.PhysicianId);
            input.HospitalId = physician.FacilityId;

            // If UnitPrice is 0, fetch from Product based on ProcedureType
            if (input.UnitPrice == 0)
            {
                input.UnitPrice = await GetProductBasePrice(input.ProductId, input.ProcedureType);
            }

            // Calculate TotalAmount if not provided or if it's automatic
            if (input.TotalAmount == 0 || input.TotalAmount == input.UnitPrice * input.Quantity)
            {
                input.TotalAmount = input.UnitPrice * input.Quantity;
            }

            var entity = new ProcedureTransaction
            {
                ProcedureDate = input.ProcedureDate,
                HospitalId = input.HospitalId,
                PhysicianId = input.PhysicianId,
                ProductId = input.ProductId,
                ProcedureType = input.ProcedureType,
                Quantity = input.Quantity,
                UnitPrice = input.UnitPrice,
                TotalAmount = input.TotalAmount
            };

            var id = await _procedureTransactionRepository.InsertAndGetIdAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await _procedureTransactionRepository.GetAll()
                .Where(pt => pt.Id == id)
                .Select(pt => new ProcedureTransactionDto
                {
                    Id = pt.Id,
                    ProcedureDate = pt.ProcedureDate,
                    HospitalId = pt.HospitalId,
                    HospitalName = pt.Hospital != null ? (pt.Hospital.FacilityName ?? "") : "",
                    PhysicianId = pt.PhysicianId,
                    PhysicianName = pt.Physician != null ? ((pt.Physician.FIRST_NAME ?? "") + " " + (pt.Physician.LAST_NAME ?? "")).Trim() : "",
                    ProductId = pt.ProductId,
                    ProductName = pt.Product != null ? (pt.Product.Name ?? "") : "",
                    ProductCode = pt.Product != null ? (pt.Product.ProductCode ?? "") : "",
                    ProcedureType = pt.ProcedureType,
                    Quantity = pt.Quantity,
                    UnitPrice = pt.UnitPrice,
                    TotalAmount = pt.TotalAmount
                })
                .FirstOrDefaultAsync();
        }

        private async Task<ProcedureTransactionDto> Update(CreateOrEditProcedureTransactionDto input)
        {
            var entity = await _procedureTransactionRepository.GetAsync(input.Id);

            // Auto-populate HospitalId from Physician's FacilityId if Physician changed
            if (entity.PhysicianId != input.PhysicianId)
            {
                var physician = await _personnelRepository.GetAsync(input.PhysicianId);
                input.HospitalId = physician.FacilityId;
            }

            entity.ProcedureDate = input.ProcedureDate;
            entity.HospitalId = input.HospitalId;
            entity.PhysicianId = input.PhysicianId;
            entity.ProductId = input.ProductId;
            entity.ProcedureType = input.ProcedureType;
            entity.Quantity = input.Quantity;
            entity.UnitPrice = input.UnitPrice;
            entity.TotalAmount = input.TotalAmount;

            await _procedureTransactionRepository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            var savedId = entity.Id;
            return await _procedureTransactionRepository.GetAll()
                .Where(pt => pt.Id == savedId)
                .Select(pt => new ProcedureTransactionDto
                {
                    Id = pt.Id,
                    ProcedureDate = pt.ProcedureDate,
                    HospitalId = pt.HospitalId,
                    HospitalName = pt.Hospital != null ? (pt.Hospital.FacilityName ?? "") : "",
                    PhysicianId = pt.PhysicianId,
                    PhysicianName = pt.Physician != null ? ((pt.Physician.FIRST_NAME ?? "") + " " + (pt.Physician.LAST_NAME ?? "")).Trim() : "",
                    ProductId = pt.ProductId,
                    ProductName = pt.Product != null ? (pt.Product.Name ?? "") : "",
                    ProductCode = pt.Product != null ? (pt.Product.ProductCode ?? "") : "",
                    ProcedureType = pt.ProcedureType,
                    Quantity = pt.Quantity,
                    UnitPrice = pt.UnitPrice,
                    TotalAmount = pt.TotalAmount
                })
                .FirstOrDefaultAsync();
        }

        public async Task Delete(EntityDto<int> input)
        {
            await _procedureTransactionRepository.DeleteAsync(input.Id);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<decimal> GetProductBasePrice(int productId, string procedureType)
        {
            var product = await _productRepository.GetAsync(productId);

            // Base price logic:
            // - For DE_NOVO (new implant with leads), use full BasePrice
            // - For GEN_CHANGE (battery replacement), use BasePrice (generator only)
            // The BasePrice in the database already accounts for this distinction
            return product.BasePrice;
        }

        public async Task<decimal> GetProductPriceByHospital(int hospitalId, int productId)
        {
            // First, try to get the hospital-specific price
            var hospitalPrice = await _hospitalProductPriceRepository.GetAll()
                .Where(hpp => hpp.HospitalId == hospitalId
                    && hpp.ProductId == productId
                    && hpp.IsActive)
                .OrderByDescending(hpp => hpp.EffectiveDate)
                .FirstOrDefaultAsync();

            if (hospitalPrice != null)
            {
                return hospitalPrice.UnitPrice;
            }

            // Fallback to product's base price if no hospital-specific price exists
            var product = await _productRepository.GetAsync(productId);
            return product.BasePrice;
        }

        private IQueryable<ProcedureTransaction> CreateFilteredQuery(GetAllProcedureTransactionsInput input)
        {
            var query = _procedureTransactionRepository.GetAll();

            // Year and Month filters
            if (input.YearFilter.HasValue)
            {
                query = query.Where(pt => pt.ProcedureDate.Year == input.YearFilter.Value);
            }

            if (input.MonthFilter.HasValue)
            {
                query = query.Where(pt => pt.ProcedureDate.Month == input.MonthFilter.Value);
            }

            // Hospital filter
            if (input.HospitalIdFilter.HasValue)
            {
                query = query.Where(pt => pt.HospitalId == input.HospitalIdFilter.Value);
            }

            // Physician filter
            if (input.PhysicianIdFilter.HasValue)
            {
                query = query.Where(pt => pt.PhysicianId == input.PhysicianIdFilter.Value);
            }

            // Product filter
            if (input.ProductIdFilter.HasValue)
            {
                query = query.Where(pt => pt.ProductId == input.ProductIdFilter.Value);
            }

            // Procedure Type filter
            if (!string.IsNullOrWhiteSpace(input.ProcedureTypeFilter))
            {
                query = query.Where(pt => pt.ProcedureType == input.ProcedureTypeFilter);
            }

            // Date range filters
            if (input.MinProcedureDateFilter.HasValue)
            {
                query = query.Where(pt => pt.ProcedureDate >= input.MinProcedureDateFilter.Value);
            }

            if (input.MaxProcedureDateFilter.HasValue)
            {
                query = query.Where(pt => pt.ProcedureDate <= input.MaxProcedureDateFilter.Value);
            }

            // General text filter
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                query = query.Where(pt =>
                    pt.Physician.FIRST_NAME.Contains(input.Filter) ||
                    pt.Physician.LAST_NAME.Contains(input.Filter) ||
                    pt.Product.Name.Contains(input.Filter) ||
                    pt.Product.ProductCode.Contains(input.Filter)
                );
            }

            return query;
        }

    }
}

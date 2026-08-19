using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using ATI.Admin.Domain.Entities;
using ATI.Revenue.Application.ProcedureTransactions.Dtos;
using ATI.Revenue.Domain.Entities;
using ATI.Revenue.Domain.Enums;
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
                ImplantType = pt.ImplantType,
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
                    ImplantType = pt.ImplantType,
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
                ImplantType = entity.ImplantType,
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
            await EnsureImplantTypeMatchesProduct(input.ProductId, input.ImplantType);

            // Auto-populate HospitalId from Physician's FacilityId
            var physician = await _personnelRepository.GetAsync(input.PhysicianId);
            input.HospitalId = physician.FacilityId;

            // If UnitPrice was not supplied, take the hospital's contracted price as of
            // the procedure date rather than the product's generic base price.
            if (input.UnitPrice == 0)
            {
                input.UnitPrice = await GetEffectiveUnitPrice(input.HospitalId, input.ProductId, input.ProcedureDate);
            }

            // Calculate TotalAmount if not provided; a supplied value is a deliberate override.
            if (input.TotalAmount == 0)
            {
                input.TotalAmount = input.UnitPrice * input.Quantity;
            }

            var entity = new ProcedureTransaction
            {
                ProcedureDate = input.ProcedureDate,
                HospitalId = input.HospitalId,
                PhysicianId = input.PhysicianId,
                ProductId = input.ProductId,
                ImplantType = input.ImplantType,
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
                    ImplantType = pt.ImplantType,
                    Quantity = pt.Quantity,
                    UnitPrice = pt.UnitPrice,
                    TotalAmount = pt.TotalAmount
                })
                .FirstOrDefaultAsync();
        }

        private async Task<ProcedureTransactionDto> Update(CreateOrEditProcedureTransactionDto input)
        {
            await EnsureImplantTypeMatchesProduct(input.ProductId, input.ImplantType);

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
            entity.ImplantType = input.ImplantType;
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
                    ImplantType = pt.ImplantType,
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

        /// <summary>
        /// Rejects a transaction whose implant type contradicts the product it records.
        /// </summary>
        /// <remarks>
        /// De Novo and Gen Change are modelled as separate product subcategories
        /// ("Single Chamber" vs "Single Chamber Gen Change"), so the product already
        /// determines which one a transaction is. Nothing previously tied the radio
        /// button to the product chosen, so a battery replacement could be recorded
        /// against a de novo device - and that mismatch would land straight in the
        /// De Novo / Gen Change columns the business reports on.
        ///
        /// Products with no subcategory cannot be checked and are left to the user.
        /// </remarks>
        private async Task EnsureImplantTypeMatchesProduct(int productId, ImplantType implantType)
        {
            var expected = await _productRepository.GetAll()
                .Where(p => p.Id == productId && p.ProductSubcategory != null)
                .Select(p => new
                {
                    ProductName = p.Name,
                    SubcategoryName = p.ProductSubcategory.SubcategoryName,
                    p.ProductSubcategory.ImplantType
                })
                .FirstOrDefaultAsync();

            if (expected == null || expected.ImplantType == implantType)
            {
                return;
            }

            throw new UserFriendlyException(
                "Implant type does not match the selected product",
                string.Format(
                    "{0} belongs to the \"{1}\" subcategory, which is {2}. Either choose a {3} product or change the implant type to {2}.",
                    expected.ProductName,
                    expected.SubcategoryName,
                    Describe(expected.ImplantType),
                    Describe(implantType)));
        }

        private static string Describe(ImplantType implantType)
        {
            return implantType == ImplantType.GenChange ? "Gen Change" : "De Novo";
        }

        /// <summary>
        /// Price the UI offers when a hospital and product are chosen. Uses today as the
        /// effective date; the server re-resolves against the procedure date on save.
        /// </summary>
        public Task<decimal> GetProductPriceByHospital(int hospitalId, int productId)
        {
            return GetEffectiveUnitPrice(hospitalId, productId, Abp.Timing.Clock.Now);
        }

        /// <summary>
        /// The hospital's contracted price for a product on a given date, falling back to
        /// the product's base price when the hospital has no contracted price.
        /// </summary>
        /// <remarks>
        /// Only prices already in effect on <paramref name="asOfDate"/> are considered, so a
        /// future-dated price increase does not retroactively reprice today's procedures.
        /// Rows with no effective date act as an undated baseline: SQL Server sorts NULLs
        /// last on a descending sort, so any dated row that has come into effect wins.
        /// </remarks>
        public async Task<decimal> GetEffectiveUnitPrice(int? hospitalId, int productId, DateTime asOfDate)
        {
            if (hospitalId.HasValue)
            {
                var hospitalPrice = await _hospitalProductPriceRepository.GetAll()
                    .Where(hpp => hpp.HospitalId == hospitalId.Value
                        && hpp.ProductId == productId
                        && hpp.IsActive
                        && (hpp.EffectiveDate == null || hpp.EffectiveDate <= asOfDate))
                    .OrderByDescending(hpp => hpp.EffectiveDate)
                    .FirstOrDefaultAsync();

                if (hospitalPrice != null)
                {
                    return hospitalPrice.UnitPrice;
                }
            }

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

            // Implant type filter (De Novo vs Gen Change)
            if (input.ImplantTypeFilter.HasValue)
            {
                query = query.Where(pt => pt.ImplantType == input.ImplantTypeFilter.Value);
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

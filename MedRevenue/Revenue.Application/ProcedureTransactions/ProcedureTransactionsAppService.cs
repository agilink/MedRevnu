using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using ATI.Admin.Domain.Entities;
using ATI.Revenue.Application.Authorization;
using ATI.Revenue.Application.ProcedureTransactions.Dtos;
using ATI.Revenue.Domain.Entities;
using ATI.Revenue.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.ProcedureTransactions
{
    /// <summary>
    /// Cases and the devices used in them.
    /// </summary>
    /// <remarks>
    /// One case is one row, identified by its case number, with a line per device. A
    /// procedure can involve two or three devices, which the previous one-product-per-row
    /// shape could not record.
    /// </remarks>
    public class ProcedureTransactionsAppService : ApplicationService, IProcedureTransactionsAppService
    {
        private readonly IRepository<ProcedureTransaction, int> _procedureTransactionRepository;
        private readonly IRepository<ProcedureTransactionProduct, int> _transactionProductRepository;
        private readonly IRepository<Personnel, int> _personnelRepository;
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<Facility, int> _facilityRepository;
        private readonly IRepository<HospitalProductPrice, int> _hospitalProductPriceRepository;
        private readonly IPhysicianDataScopeProvider _physicianDataScopeProvider;

        public ProcedureTransactionsAppService(
            IRepository<ProcedureTransaction, int> procedureTransactionRepository,
            IRepository<ProcedureTransactionProduct, int> transactionProductRepository,
            IRepository<Personnel, int> personnelRepository,
            IRepository<Product, int> productRepository,
            IRepository<Facility, int> facilityRepository,
            IRepository<HospitalProductPrice, int> hospitalProductPriceRepository,
            IPhysicianDataScopeProvider physicianDataScopeProvider)
        {
            _procedureTransactionRepository = procedureTransactionRepository;
            _transactionProductRepository = transactionProductRepository;
            _personnelRepository = personnelRepository;
            _productRepository = productRepository;
            _facilityRepository = facilityRepository;
            _hospitalProductPriceRepository = hospitalProductPriceRepository;
            _physicianDataScopeProvider = physicianDataScopeProvider;
        }

        public async Task<PagedResultDto<ProcedureTransactionDto>> GetAll(GetAllProcedureTransactionsInput input)
        {
            // A physician sees only their own hospital's cases, whatever hospital the page
            // asked for, and nothing at all until a hospital is assigned to them.
            var scope = await _physicianDataScopeProvider.GetAsync();

            if (scope.SeesNothing)
            {
                return new PagedResultDto<ProcedureTransactionDto>(0, new List<ProcedureTransactionDto>());
            }

            if (scope.IsRestricted)
            {
                input.HospitalIdFilter = scope.HospitalId;
            }

            var query = CreateFilteredQuery(input);

            var totalCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);
            else
                query = query.OrderByDescending(pt => pt.ProcedureDate);

            var rows = await query.PageBy(input)
                .Select(pt => new
                {
                    pt.Id,
                    pt.CaseNumber,
                    pt.ProcedureDate,
                    pt.HospitalId,
                    HospitalName = pt.Hospital != null ? (pt.Hospital.FacilityName ?? "") : "",
                    pt.PhysicianId,
                    PhysicianName = pt.Physician != null
                        ? ((pt.Physician.FIRST_NAME ?? "") + " " + (pt.Physician.LAST_NAME ?? "")).Trim()
                        : "",
                    pt.ImplantType,
                    pt.Status,
                    pt.Description,
                    pt.TotalAmount,
                    Lines = pt.Products.Select(l => new
                    {
                        ProductName = l.Product != null ? (l.Product.Name ?? "") : "",
                        ProductCode = l.Product != null ? (l.Product.ProductCode ?? "") : "",
                        l.Quantity
                    }).ToList()
                })
                .ToListAsync();

            var dtos = rows.Select(r => new ProcedureTransactionDto
            {
                Id = r.Id,
                CaseNumber = r.CaseNumber,
                ProcedureDate = r.ProcedureDate,
                HospitalId = r.HospitalId,
                HospitalName = r.HospitalName,
                PhysicianId = r.PhysicianId,
                PhysicianName = r.PhysicianName,
                ImplantType = r.ImplantType,
                Status = r.Status,
                Description = r.Description ?? "",
                TotalAmount = r.TotalAmount,
                DeviceCount = r.Lines.Count,
                TotalUnits = r.Lines.Sum(l => l.Quantity),
                ProductSummary = Summarise(r.Lines.Select(l =>
                    string.IsNullOrWhiteSpace(l.ProductCode) ? l.ProductName : l.ProductCode))
            }).ToList();

            return new PagedResultDto<ProcedureTransactionDto>(totalCount, dtos);
        }

        /// <summary>
        /// Stops a physician reaching a case that belongs to another hospital.
        /// </summary>
        /// <remarks>
        /// Filtering the list is not enough on its own: view, edit and delete all take a
        /// case id, so without this a physician could reach any case by guessing an id.
        /// </remarks>
        private async Task EnsureCaseIsInScope(int caseId)
        {
            var scope = await _physicianDataScopeProvider.GetAsync();

            if (!scope.IsRestricted)
            {
                return;
            }

            var hospitalId = await _procedureTransactionRepository.GetAll()
                .Where(pt => pt.Id == caseId)
                .Select(pt => pt.HospitalId)
                .FirstOrDefaultAsync();

            if (!scope.HospitalId.HasValue || hospitalId != scope.HospitalId.Value)
            {
                throw new UserFriendlyException(L("CaseBelongsToAnotherHospital"));
            }
        }

        /// <summary>
        /// Forces a physician's case onto their own hospital rather than trusting the form.
        /// </summary>
        private async Task<CreateOrEditProcedureTransactionDto> ApplyScopeToInput(
            CreateOrEditProcedureTransactionDto input)
        {
            var scope = await _physicianDataScopeProvider.GetAsync();

            if (!scope.IsRestricted)
            {
                return input;
            }

            if (!scope.HospitalId.HasValue)
            {
                throw new UserFriendlyException(L("NoHospitalAssignedToPhysicianUser"));
            }

            // Id 0 means a new case; an existing one has to belong to their hospital
            // before they are allowed to move it.
            if (input.Id != 0)
            {
                await EnsureCaseIsInScope(input.Id);
            }

            input.HospitalId = scope.HospitalId;

            return input;
        }

        public async Task<GetProcedureTransactionForViewDto> GetProcedureTransactionForView(int id)
        {
            await EnsureCaseIsInScope(id);

            var dto = await ProjectWithLines(_procedureTransactionRepository.GetAll().Where(pt => pt.Id == id));

            if (dto == null)
                throw new UserFriendlyException("Case not found");

            return new GetProcedureTransactionForViewDto { ProcedureTransaction = dto };
        }

        public async Task<GetProcedureTransactionForEditOutput> GetProcedureTransactionForEdit(EntityDto<int> input)
        {
            await EnsureCaseIsInScope(input.Id);

            var entity = await _procedureTransactionRepository.GetAll()
                .Where(pt => pt.Id == input.Id)
                .Select(pt => new
                {
                    pt.Id,
                    pt.CaseNumber,
                    pt.ProcedureDate,
                    pt.HospitalId,
                    pt.PhysicianId,
                    pt.ImplantType,
                    pt.Status,
                    pt.Description,
                    pt.TotalAmount,
                    Lines = pt.Products.Select(l => new CreateOrEditProcedureTransactionProductDto
                    {
                        Id = l.Id,
                        ProductId = l.ProductId,
                        Quantity = l.Quantity,
                        UnitPrice = l.UnitPrice
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (entity == null)
                throw new UserFriendlyException("Case not found");

            return new GetProcedureTransactionForEditOutput
            {
                ProcedureTransaction = new CreateOrEditProcedureTransactionDto
                {
                    Id = entity.Id,
                    CaseNumber = entity.CaseNumber,
                    ProcedureDate = entity.ProcedureDate,
                    HospitalId = entity.HospitalId,
                    PhysicianId = entity.PhysicianId,
                    ImplantType = entity.ImplantType,
                    Status = entity.Status,
                    Description = entity.Description,
                    TotalAmount = entity.TotalAmount,
                    Products = entity.Lines
                }
            };
        }

        public async Task<ProcedureTransactionDto> CreateOrEdit(CreateOrEditProcedureTransactionDto input)
        {
            if (input.Products == null || !input.Products.Any())
            {
                throw new UserFriendlyException("A case needs at least one device.");
            }

            input = await ApplyScopeToInput(input);

            input.CaseNumber = input.CaseNumber?.Trim();
            await EnsureCaseNumberIsUnique(input.Id, input.CaseNumber);
            await EnsureEveryDeviceMatchesImplantType(input.Products, input.ImplantType);

            try
            {
                return await SaveCase(input);
            }
            catch (Exception exception) when (IsCaseNumberConflict(exception))
            {
                // Two saves at once: the pre-check passed for both and the index caught
                // the loser.
                throw new UserFriendlyException($"Case number \"{input.CaseNumber}\" is already in use.");
            }
        }

        private async Task<ProcedureTransactionDto> SaveCase(CreateOrEditProcedureTransactionDto input)
        {

            // The hospital drives the case: it is chosen first, narrows the physician list,
            // and the devices are priced against it. Only when it was left blank is it
            // taken from the physician's facility - previously it was overwritten from the
            // physician unconditionally, which silently discarded the hospital chosen.
            if (!input.HospitalId.HasValue)
            {
                var physician = await _personnelRepository.GetAsync(input.PhysicianId);
                input.HospitalId = physician.FacilityId;
            }

            var entity = input.Id == 0
                ? new ProcedureTransaction()
                : await _procedureTransactionRepository.GetAllIncluding(pt => pt.Products)
                      .FirstOrDefaultAsync(pt => pt.Id == input.Id);

            if (entity == null)
                throw new UserFriendlyException("Case not found");

            entity.CaseNumber = input.CaseNumber;
            entity.ProcedureDate = input.ProcedureDate;
            entity.HospitalId = input.HospitalId;
            entity.PhysicianId = input.PhysicianId;
            entity.ImplantType = input.ImplantType;
            entity.Status = input.Status;
            entity.Description = input.Description;

            await ApplyLines(entity, input);

            // A supplied total is a deliberate override; otherwise it is the sum of lines.
            entity.TotalAmount = input.TotalAmount > 0
                ? input.TotalAmount
                : entity.Products.Sum(l => l.LineTotal);

            int id;
            if (input.Id == 0)
            {
                id = await _procedureTransactionRepository.InsertAndGetIdAsync(entity);
            }
            else
            {
                await _procedureTransactionRepository.UpdateAsync(entity);
                id = entity.Id;
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return await ProjectWithLines(_procedureTransactionRepository.GetAll().Where(pt => pt.Id == id));
        }

        /// <summary>
        /// Replaces the case's device lines with the supplied set, pricing each one from
        /// the hospital's contracted price unless a price was given.
        /// </summary>
        private async Task ApplyLines(ProcedureTransaction entity, CreateOrEditProcedureTransactionDto input)
        {
            var keptIds = input.Products.Where(l => l.Id > 0).Select(l => l.Id).ToHashSet();

            foreach (var removed in entity.Products.Where(l => !keptIds.Contains(l.Id)).ToList())
            {
                entity.Products.Remove(removed);
                await _transactionProductRepository.DeleteAsync(removed);
            }

            foreach (var line in input.Products)
            {
                var unitPrice = line.UnitPrice > 0
                    ? line.UnitPrice
                    : await GetEffectiveUnitPrice(input.HospitalId, line.ProductId, input.ProcedureDate);

                var quantity = line.Quantity > 0 ? line.Quantity : 1;

                var existing = line.Id > 0
                    ? entity.Products.FirstOrDefault(l => l.Id == line.Id)
                    : null;

                if (existing == null)
                {
                    entity.Products.Add(new ProcedureTransactionProduct
                    {
                        ProductId = line.ProductId,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        LineTotal = unitPrice * quantity
                    });
                }
                else
                {
                    existing.ProductId = line.ProductId;
                    existing.Quantity = quantity;
                    existing.UnitPrice = unitPrice;
                    existing.LineTotal = unitPrice * quantity;
                }
            }
        }

        public async Task Delete(EntityDto<int> input)
        {
            await EnsureCaseIsInScope(input.Id);

            await _procedureTransactionRepository.DeleteAsync(input.Id);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Turns the database's unique-index violation on CaseNumber into the same
        /// friendly message the pre-check gives.
        /// </summary>
        /// <remarks>
        /// EnsureCaseNumberIsUnique cannot be airtight on its own: two saves submitted at
        /// once both pass the check and the second then trips the index, which surfaced as
        /// a raw DbUpdateException ("An error occurred while saving the entity changes").
        /// The index is the real guarantee, so its violation is translated rather than
        /// leaked.
        /// </remarks>
        private static bool IsCaseNumberConflict(Exception exception)
        {
            for (var current = exception; current != null; current = current.InnerException)
            {
                if (current is SqlException sql
                    && (sql.Number == 2601 || sql.Number == 2627)
                    && sql.Message.IndexOf("IX_ProcedureTransaction_CaseNumber",
                                           StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task EnsureCaseNumberIsUnique(int id, string caseNumber)
        {
            if (string.IsNullOrWhiteSpace(caseNumber))
            {
                throw new UserFriendlyException("A case number is required.");
            }

            var taken = await _procedureTransactionRepository.GetAll()
                .AnyAsync(pt => pt.Id != id && pt.CaseNumber == caseNumber);

            if (taken)
            {
                throw new UserFriendlyException($"Case number \"{caseNumber}\" is already in use.");
            }
        }

        /// <summary>
        /// Rejects any device whose subcategory contradicts the case's implant type.
        /// </summary>
        /// <remarks>
        /// De Novo and Gen Change are separate product subcategories ("Single Chamber" vs
        /// "Single Chamber Gen Change"), so a product determines which it is. Every device
        /// on a case must agree with the case, otherwise the De Novo / Gen Change columns
        /// the business reports on would be wrong. Products with no subcategory cannot be
        /// checked and are left to the user.
        /// </remarks>
        private async Task EnsureEveryDeviceMatchesImplantType(
            List<CreateOrEditProcedureTransactionProductDto> lines,
            ImplantType implantType)
        {
            var productIds = lines.Select(l => l.ProductId).Distinct().ToList();

            var mismatches = await _productRepository.GetAll()
                .Where(p => productIds.Contains(p.Id)
                            && p.ProductSubcategory != null
                            && p.ProductSubcategory.ImplantType != implantType)
                .Select(p => new
                {
                    p.Name,
                    SubcategoryName = p.ProductSubcategory.SubcategoryName,
                    p.ProductSubcategory.ImplantType
                })
                .ToListAsync();

            if (!mismatches.Any())
            {
                return;
            }

            var detail = string.Join("; ", mismatches.Select(m =>
                $"{m.Name} is in \"{m.SubcategoryName}\", which is {Describe(m.ImplantType)}"));

            throw new UserFriendlyException(
                $"Every device on a {Describe(implantType)} case must be {Describe(implantType)}",
                detail + ". Remove those devices or change the case's implant type.");
        }

        private static string Describe(ImplantType implantType)
        {
            return implantType == ImplantType.GenChange ? "Gen Change" : "De Novo";
        }

        private static string Summarise(IEnumerable<string> names)
        {
            var list = names.Where(n => !string.IsNullOrWhiteSpace(n)).ToList();

            if (!list.Any()) return "";
            if (list.Count == 1) return list[0];

            return $"{list[0]} +{list.Count - 1} more";
        }

        private static Task<ProcedureTransactionDto> ProjectWithLines(IQueryable<ProcedureTransaction> query)
        {
            return query.Select(pt => new ProcedureTransactionDto
            {
                Id = pt.Id,
                CaseNumber = pt.CaseNumber,
                Status = pt.Status,
                Description = pt.Description ?? "",
                ProcedureDate = pt.ProcedureDate,
                HospitalId = pt.HospitalId,
                HospitalName = pt.Hospital != null ? (pt.Hospital.FacilityName ?? "") : "",
                PhysicianId = pt.PhysicianId,
                PhysicianName = pt.Physician != null
                    ? ((pt.Physician.FIRST_NAME ?? "") + " " + (pt.Physician.LAST_NAME ?? "")).Trim()
                    : "",
                ImplantType = pt.ImplantType,
                TotalAmount = pt.TotalAmount,
                DeviceCount = pt.Products.Count,
                TotalUnits = pt.Products.Sum(l => l.Quantity),
                Products = pt.Products.Select(l => new ProcedureTransactionProductDto
                {
                    Id = l.Id,
                    ProductId = l.ProductId,
                    ProductName = l.Product != null ? (l.Product.Name ?? "") : "",
                    ProductCode = l.Product != null ? (l.Product.ProductCode ?? "") : "",
                    SubcategoryName = l.Product != null && l.Product.ProductSubcategory != null
                        ? (l.Product.ProductSubcategory.SubcategoryName ?? "")
                        : "",
                    ProductImplantType = l.Product != null && l.Product.ProductSubcategory != null
                        ? l.Product.ProductSubcategory.ImplantType
                        : (ImplantType?)null,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    LineTotal = l.LineTotal
                }).ToList()
            }).FirstOrDefaultAsync();
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
        public async Task<decimal> GetEffectiveUnitPrice(int? hospitalId, int productId, DateTime? asOfDate = null)
        {
            // The form may ask before a date has been entered.
            var asOf = asOfDate ?? Abp.Timing.Clock.Now;

            if (hospitalId.HasValue)
            {
                var hospitalPrice = await _hospitalProductPriceRepository.GetAll()
                    .Where(hpp => hpp.HospitalId == hospitalId.Value
                        && hpp.ProductId == productId
                        && hpp.IsActive
                        && (hpp.EffectiveDate == null || hpp.EffectiveDate <= asOf))
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
            return _procedureTransactionRepository.GetAll()
                .WhereIf(input.YearFilter.HasValue, pt => pt.ProcedureDate.Year == input.YearFilter.Value)
                .WhereIf(input.MonthFilter.HasValue, pt => pt.ProcedureDate.Month == input.MonthFilter.Value)
                .WhereIf(input.HospitalIdFilter.HasValue, pt => pt.HospitalId == input.HospitalIdFilter.Value)
                .WhereIf(input.PhysicianIdFilter.HasValue, pt => pt.PhysicianId == input.PhysicianIdFilter.Value)
                // A case matches a product filter when any of its devices is that product.
                .WhereIf(input.ProductIdFilter.HasValue,
                    pt => pt.Products.Any(l => l.ProductId == input.ProductIdFilter.Value))
                .WhereIf(input.ImplantTypeFilter.HasValue, pt => pt.ImplantType == input.ImplantTypeFilter.Value)
                .WhereIf(input.MinProcedureDateFilter.HasValue, pt => pt.ProcedureDate >= input.MinProcedureDateFilter.Value)
                .WhereIf(input.MaxProcedureDateFilter.HasValue, pt => pt.ProcedureDate <= input.MaxProcedureDateFilter.Value)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), pt =>
                    pt.CaseNumber.Contains(input.Filter) ||
                    pt.Physician.FIRST_NAME.Contains(input.Filter) ||
                    pt.Physician.LAST_NAME.Contains(input.Filter) ||
                    pt.Products.Any(l => l.Product.Name.Contains(input.Filter)
                                         || l.Product.ProductCode.Contains(input.Filter)));
        }
    }
}

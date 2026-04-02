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

        public ProcedureTransactionsAppService(
            IRepository<ProcedureTransaction, int> procedureTransactionRepository,
            IRepository<Personnel, int> personnelRepository,
            IRepository<Product, int> productRepository,
            IRepository<Facility, int> facilityRepository)
        {
            _procedureTransactionRepository = procedureTransactionRepository;
            _personnelRepository = personnelRepository;
            _productRepository = productRepository;
            _facilityRepository = facilityRepository;
        }

        public async Task<PagedResultDto<ProcedureTransactionDto>> GetAll(GetAllProcedureTransactionsInput input)
        {
            var query = CreateFilteredQuery(input)
                .Include(pt => pt.Hospital)
                .Include(pt => pt.Physician)
                .Include(pt => pt.Product)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                query = query.OrderBy(input.Sorting);
            }
            else
            {
                query = query.OrderByDescending(pt => pt.ProcedureDate);
            }

            // Apply paging
            query = query.PageBy(input);

            var entities = await query.ToListAsync();
            var dtos = entities.Select(entity => new ProcedureTransactionDto
            {
                Id = entity.Id,
                ProcedureDate = entity.ProcedureDate,
                HospitalId = entity.HospitalId,
                HospitalName = entity.Hospital?.FacilityName ?? "",
                PhysicianId = entity.PhysicianId,
                PhysicianName = GetPhysicianFullName(entity.Physician),
                ProductId = entity.ProductId,
                ProductName = entity.Product?.Name ?? "",
                ProductCode = entity.Product?.ProductCode ?? "",
                ProcedureType = entity.ProcedureType,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                TotalAmount = entity.TotalAmount
            }).ToList();

            return new PagedResultDto<ProcedureTransactionDto>(totalCount, dtos);
        }

        public async Task<GetProcedureTransactionForViewDto> GetProcedureTransactionForView(int id)
        {
            var entity = await _procedureTransactionRepository
                .GetAll()
                .Include(pt => pt.Hospital)
                .Include(pt => pt.Physician)
                .Include(pt => pt.Product)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entity == null)
            {
                throw new UserFriendlyException("Procedure Transaction not found");
            }

            var dto = new ProcedureTransactionDto
            {
                Id = entity.Id,
                ProcedureDate = entity.ProcedureDate,
                HospitalId = entity.HospitalId,
                HospitalName = entity.Hospital?.FacilityName ?? "",
                PhysicianId = entity.PhysicianId,
                PhysicianName = GetPhysicianFullName(entity.Physician),
                ProductId = entity.ProductId,
                ProductName = entity.Product?.Name ?? "",
                ProductCode = entity.Product?.ProductCode ?? "",
                ProcedureType = entity.ProcedureType,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                TotalAmount = entity.TotalAmount
            };

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

            // Load navigation properties for return DTO
            entity = await _procedureTransactionRepository
                .GetAll()
                .Include(pt => pt.Hospital)
                .Include(pt => pt.Physician)
                .Include(pt => pt.Product)
                .FirstOrDefaultAsync(e => e.Id == id);

            return new ProcedureTransactionDto
            {
                Id = entity.Id,
                ProcedureDate = entity.ProcedureDate,
                HospitalId = entity.HospitalId,
                HospitalName = entity.Hospital?.FacilityName ?? "",
                PhysicianId = entity.PhysicianId,
                PhysicianName = GetPhysicianFullName(entity.Physician),
                ProductId = entity.ProductId,
                ProductName = entity.Product?.Name ?? "",
                ProductCode = entity.Product?.ProductCode ?? "",
                ProcedureType = entity.ProcedureType,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                TotalAmount = entity.TotalAmount
            };
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

            // Load navigation properties for return DTO
            entity = await _procedureTransactionRepository
                .GetAll()
                .Include(pt => pt.Hospital)
                .Include(pt => pt.Physician)
                .Include(pt => pt.Product)
                .FirstOrDefaultAsync(e => e.Id == entity.Id);

            return new ProcedureTransactionDto
            {
                Id = entity.Id,
                ProcedureDate = entity.ProcedureDate,
                HospitalId = entity.HospitalId,
                HospitalName = entity.Hospital?.FacilityName ?? "",
                PhysicianId = entity.PhysicianId,
                PhysicianName = GetPhysicianFullName(entity.Physician),
                ProductId = entity.ProductId,
                ProductName = entity.Product?.Name ?? "",
                ProductCode = entity.Product?.ProductCode ?? "",
                ProcedureType = entity.ProcedureType,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                TotalAmount = entity.TotalAmount
            };
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

        private string GetPhysicianFullName(Personnel physician)
        {
            if (physician == null) return "";

            var firstName = physician.FIRST_NAME ?? "";
            var lastName = physician.LAST_NAME ?? "";
            return $"{firstName} {lastName}".Trim();
        }
    }
}

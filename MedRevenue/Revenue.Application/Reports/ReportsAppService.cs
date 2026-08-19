using Abp.Application.Services;
using Abp.Domain.Repositories;
using ATI.Admin.Domain.Entities;
using ATI.Revenue.Application.Reports.Dtos;
using ATI.Revenue.Domain.Entities;
using ATI.Revenue.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Reports
{
    public class ReportsAppService : ApplicationService, IReportsAppService
    {
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<ProductCategory, int> _productCategoryRepository;
        private readonly IRepository<ProcedureTransaction, int> _procedureTransactionRepository;
        private readonly IRepository<Personnel, int> _personnelRepository;
        private readonly IRepository<Facility, int> _facilityRepository;

        public ReportsAppService(
            IRepository<Product, int> productRepository,
            IRepository<ProductCategory, int> productCategoryRepository,
            IRepository<ProcedureTransaction, int> procedureTransactionRepository,
            IRepository<Personnel, int> personnelRepository,
            IRepository<Facility, int> facilityRepository)
        {
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _procedureTransactionRepository = procedureTransactionRepository;
            _personnelRepository = personnelRepository;
            _facilityRepository = facilityRepository;
        }

        /// <summary>
        /// Report 1: Rate Chart - Products by ProductCategory with prices for a hospital
        /// </summary>
        public async Task<List<RateChartReportDto>> GetRateChartReport(RateChartReportInput input)
        {
            var products = await _productRepository
                .GetAll()
                .Include(p => p.ProductCategory)
                .Where(p => p.IsActive && p.ProductCategory != null)
                .OrderBy(p => p.ProductCategory.Name)
                .ThenBy(p => p.Name)
                .Select(p => new RateChartReportDto
                {
                    ProductCategoryName = p.ProductCategory.Name,
                    ProductCode = p.ProductCode,
                    ProductName = p.Name,
                    BasePrice = p.BasePrice,
                    IsSystem = p.IsSystem
                })
                .ToListAsync();

            return products;
        }

        /// <summary>
        /// Report 2: Monthly Revenue - Daily revenue by ProductCategory for each month
        /// </summary>
        public async Task<List<MonthlyRevenueReportDto>> GetMonthlyRevenueReport(MonthlyRevenueReportInput input)
        {
            var query = _procedureTransactionRepository
                .GetAll()
                .Include(pt => pt.Product)
                    .ThenInclude(p => p.ProductCategory)
                .Where(pt => pt.ProcedureDate.Year == input.Year &&
                             pt.ProcedureDate.Month == input.Month);

            if (input.HospitalId.HasValue)
            {
                query = query.Where(pt => pt.HospitalId == input.HospitalId.Value);
            }

            var report = await query
                .GroupBy(pt => new
                {
                    Date = pt.ProcedureDate.Date,
                    CategoryName = pt.Product.ProductCategory.Name
                })
                .Select(g => new MonthlyRevenueReportDto
                {
                    ProcedureDate = g.Key.Date,
                    ProductCategoryName = g.Key.CategoryName,
                    DailyRevenue = g.Sum(pt => pt.TotalAmount),
                    TransactionCount = g.Count()
                })
                .OrderBy(r => r.ProcedureDate)
                .ThenBy(r => r.ProductCategoryName)
                .ToListAsync();

            return report;
        }

        /// <summary>
        /// Report 3: Cases by Person - Cases per physician for specified year
        /// </summary>
        public async Task<List<CasesByPersonReportDto>> GetCasesByPersonReport(CasesByPersonReportInput input)
        {
            var query = _procedureTransactionRepository
                .GetAll()
                .Include(pt => pt.Physician)
                    .ThenInclude(p => p.Facility)
                .Where(pt => pt.ProcedureDate.Year == input.Year);

            if (input.HospitalId.HasValue)
            {
                query = query.Where(pt => pt.HospitalId == input.HospitalId.Value);
            }

            if (input.PhysicianId.HasValue)
            {
                query = query.Where(pt => pt.PhysicianId == input.PhysicianId.Value);
            }

            var report = await query
                .GroupBy(pt => new
                {
                    PhysicianId = pt.PhysicianId,
                    PhysicianFirstName = pt.Physician.FIRST_NAME,
                    PhysicianLastName = pt.Physician.LAST_NAME,
                    HospitalName = pt.Hospital != null ? pt.Hospital.FacilityName : ""
                })
                .Select(g => new CasesByPersonReportDto
                {
                    PhysicianName = (g.Key.PhysicianFirstName + " " + g.Key.PhysicianLastName).Trim(),
                    HospitalName = g.Key.HospitalName,
                    TotalCases = g.Sum(pt => pt.Quantity),
                    DeNovoCases = g.Where(pt => pt.ImplantType == ImplantType.DeNovo).Sum(pt => pt.Quantity),
                    GenChangeCases = g.Where(pt => pt.ImplantType == ImplantType.GenChange).Sum(pt => pt.Quantity),
                    TotalRevenue = g.Sum(pt => pt.TotalAmount)
                })
                .OrderByDescending(r => r.TotalCases)
                .ToListAsync();

            return report;
        }

        /// <summary>
        /// Report 4: Transaction Amount - By physician per product type for specified year
        /// </summary>
        public async Task<List<TransactionAmountReportDto>> GetTransactionAmountReport(TransactionAmountReportInput input)
        {
            var query = _procedureTransactionRepository
                .GetAll()
                .Include(pt => pt.Physician)
                .Include(pt => pt.Product)
                    .ThenInclude(p => p.ProductCategory)
                .Where(pt => pt.ProcedureDate.Year == input.Year);

            if (input.PhysicianId.HasValue)
            {
                query = query.Where(pt => pt.PhysicianId == input.PhysicianId.Value);
            }

            if (input.ProductCategoryId.HasValue)
            {
                query = query.Where(pt => pt.Product.ProductCategoryId == input.ProductCategoryId.Value);
            }

            var report = await query
                .GroupBy(pt => new
                {
                    PhysicianFirstName = pt.Physician.FIRST_NAME,
                    PhysicianLastName = pt.Physician.LAST_NAME,
                    CategoryName = pt.Product.ProductCategory.Name,
                    ImplantType = pt.ImplantType
                })
                .Select(g => new TransactionAmountReportDto
                {
                    PhysicianName = (g.Key.PhysicianFirstName + " " + g.Key.PhysicianLastName).Trim(),
                    ProductCategoryName = g.Key.CategoryName,
                    ImplantType = g.Key.ImplantType,
                    TotalCases = g.Sum(pt => pt.Quantity),
                    TotalAmount = g.Sum(pt => pt.TotalAmount),
                    AverageAmount = g.Average(pt => pt.TotalAmount)
                })
                .OrderBy(r => r.PhysicianName)
                .ThenBy(r => r.ProductCategoryName)
                .ThenBy(r => r.ImplantType)
                .ToListAsync();

            return report;
        }
    }
}

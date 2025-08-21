using Abp.Domain.Services;
using ATI.MedRevnu.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ATI.MedRevnu.Domain.Services
{
    /// <summary>
    /// Domain service for calculating quotas and revenue metrics
    /// </summary>
    public class QuotaCalculationService : DomainService
    {
        public decimal CalculateMonthlyRevenue(IEnumerable<Case> cases, int month, int year)
        {
            return cases
                .Where(c => c.Date.Month == month && c.Date.Year == year)
                .Sum(c => c.GetTotalRevenue());
        }

        public decimal CalculateQuarterlyRevenue(IEnumerable<Case> cases, int quarter, int year)
        {
            var startMonth = (quarter - 1) * 3 + 1;
            var endMonth = startMonth + 2;

            return cases
                .Where(c => c.Date.Year == year && 
                           c.Date.Month >= startMonth && 
                           c.Date.Month <= endMonth)
                .Sum(c => c.GetTotalRevenue());
        }

        public decimal CalculateYearlyRevenue(IEnumerable<Case> cases, int year)
        {
            return cases
                .Where(c => c.Date.Year == year)
                .Sum(c => c.GetTotalRevenue());
        }

        public Dictionary<string, decimal> GetRevenueByCategoryForPeriod(
            IEnumerable<Case> cases, 
            DateTime startDate, 
            DateTime endDate)
        {
            var casesInPeriod = cases
                .Where(c => c.Date >= startDate && c.Date <= endDate)
                .ToList();

            var result = new Dictionary<string, decimal>();

            foreach (var caseItem in casesInPeriod)
            {
                foreach (var caseProduct in caseItem.CaseProducts)
                {
                    var category = caseProduct.Product?.Category ?? "Unknown";
                    
                    if (!result.ContainsKey(category))
                        result[category] = 0;
                    
                    result[category] += caseProduct.Revenue;
                }
            }

            return result;
        }

        public Dictionary<string, int> GetCaseCountByDoctorForPeriod(
            IEnumerable<Case> cases, 
            DateTime startDate, 
            DateTime endDate)
        {
            return cases
                .Where(c => c.Date >= startDate && c.Date <= endDate)
                .GroupBy(c => c.Doctor?.FullName ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public decimal CalculateQuotaPercentage(decimal actualRevenue, decimal targetRevenue)
        {
            return targetRevenue > 0 ? (actualRevenue / targetRevenue) * 100 : 0;
        }

        public bool IsQuotaMet(decimal actualRevenue, decimal targetRevenue)
        {
            return actualRevenue >= targetRevenue;
        }

        public decimal GetAverageRevenuePerCase(IEnumerable<Case> cases)
        {
            var casesList = cases.ToList();
            if (!casesList.Any()) return 0;

            return casesList.Sum(c => c.GetTotalRevenue()) / casesList.Count;
        }

        public Dictionary<string, decimal> GetTopProductsByRevenue(
            IEnumerable<Case> cases, 
            int topCount = 10)
        {
            return cases
                .SelectMany(c => c.CaseProducts)
                .GroupBy(cp => cp.Product?.Name ?? "Unknown")
                .Select(g => new { ProductName = g.Key, TotalRevenue = g.Sum(cp => cp.Revenue) })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(topCount)
                .ToDictionary(x => x.ProductName, x => x.TotalRevenue);
        }
    }
}
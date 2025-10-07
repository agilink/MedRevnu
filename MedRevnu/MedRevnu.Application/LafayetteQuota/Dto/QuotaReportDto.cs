using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ATI.MedRevnu.Application.LafayetteQuota.Dto
{
    public class QuotaReportDto
    {
        public DateTime ReportDate { get; set; }
        public string ReportType { get; set; }
        public string Period { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TargetRevenue { get; set; }
        public decimal QuotaPercentage { get; set; }
        public bool IsQuotaMet { get; set; }
        public int TotalCases { get; set; }
        public decimal AverageRevenuePerCase { get; set; }
        public Dictionary<string, decimal> RevenueByCategory { get; set; }
        public Dictionary<string, int> CasesByDoctor { get; set; }
        public Dictionary<string, decimal> TopProductsByRevenue { get; set; }
    }

    public class GetQuotaReportInput
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [StringLength(50)]
        public string ReportType { get; set; }

        public int? DoctorId { get; set; }

        public int? HospitalId { get; set; }

        public int? FacilityId { get; set; }

        public string Category { get; set; }

        public decimal? TargetRevenue { get; set; }

        public GetQuotaReportInput()
        {
            ReportType = "Monthly";
        }
    }

    public class MonthlyQuotaReportDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; }
        public decimal Revenue { get; set; }
        public decimal TargetRevenue { get; set; }
        public decimal QuotaPercentage { get; set; }
        public bool IsQuotaMet { get; set; }
        public int CaseCount { get; set; }
        public Dictionary<string, decimal> CategoryBreakdown { get; set; }
    }

    public class QuarterlyQuotaReportDto
    {
        public int Quarter { get; set; }
        public int Year { get; set; }
        public string QuarterName { get; set; }
        public decimal Revenue { get; set; }
        public decimal TargetRevenue { get; set; }
        public decimal QuotaPercentage { get; set; }
        public bool IsQuotaMet { get; set; }
        public int CaseCount { get; set; }
        public List<MonthlyQuotaReportDto> MonthlyBreakdown { get; set; }
    }

    public class DoctorPerformanceReportDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Specialty { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCases { get; set; }
        public decimal AverageRevenuePerCase { get; set; }
        public Dictionary<string, decimal> RevenueByCategory { get; set; }
        public Dictionary<string, int> CasesByProcedureType { get; set; }
    }

    public class GetDoctorPerformanceReportInput
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int? DoctorId { get; set; }

        public int? HospitalId { get; set; }

        public int TopCount { get; set; }

        public GetDoctorPerformanceReportInput()
        {
            TopCount = 10;
        }
    }

    public class ProductPerformanceReportDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string ModelNo { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalQuantity { get; set; }
        public decimal AverageRevenuePerUnit { get; set; }
        public int UsageCount { get; set; }
        public decimal MarketShare { get; set; }
    }

    public class GetProductPerformanceReportInput
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int? ProductId { get; set; }

        public string Category { get; set; }

        public int TopCount { get; set; }

        public GetProductPerformanceReportInput()
        {
            TopCount = 10;
        }
    }
}
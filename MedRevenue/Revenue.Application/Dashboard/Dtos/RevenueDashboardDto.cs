using System;
using System.Collections.Generic;

namespace ATI.Revenue.Application.Dashboard.Dtos
{
    /// <summary>
    /// Everything the Revenue dashboard shows, for one date range, in a single call.
    /// </summary>
    /// <remarks>
    /// Two different grains are deliberately mixed, because the questions differ:
    ///  - the headline figures and the top-10 lists use the case's TotalAmount, so a
    ///    negotiated case price is respected;
    ///  - the device-type breakdown uses the device lines, because one case can span more
    ///    than one product category and quotas are set per category.
    /// Where a case total has been overridden, the sum of the device-type rows can
    /// therefore differ slightly from Total Sold. That is the honest consequence of
    /// allowing an override, not a rounding error.
    /// </remarks>
    public class RevenueDashboardDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public int TotalCases { get; set; }
        public int TotalUnits { get; set; }
        public decimal TotalSold { get; set; }

        /// <summary>
        /// Sum of the monthly targets for every month the range touches. A partial month
        /// still contributes its whole target - targets are set per month and there is no
        /// basis for splitting one across days.
        /// </summary>
        public decimal TotalPlanned { get; set; }

        public decimal Variance { get; set; }
        public decimal PercentAchieved { get; set; }
        public bool HasPlan { get; set; }

        /// <summary>Which months the planned figure was taken from, for the caption.</summary>
        public List<string> PlannedMonths { get; set; } = new List<string>();

        public List<DeviceTypePerformanceDto> DeviceTypes { get; set; } = new List<DeviceTypePerformanceDto>();
        public List<CollectionRowDto> TopPhysicians { get; set; } = new List<CollectionRowDto>();
        public List<CollectionRowDto> TopHospitals { get; set; } = new List<CollectionRowDto>();
        public List<DailyRevenueRowDto> DailyRevenue { get; set; } = new List<DailyRevenueRowDto>();
    }

    /// <summary>Sold against planned for one product category.</summary>
    public class DeviceTypePerformanceDto
    {
        public int ProductCategoryId { get; set; }
        public string DeviceType { get; set; }
        public decimal Sold { get; set; }
        public decimal Planned { get; set; }
        public decimal Variance { get; set; }
        public decimal PercentAchieved { get; set; }
        public int Cases { get; set; }
        public int Units { get; set; }

        /// <summary>False where revenue was recorded with no target covering it.</summary>
        public bool HasPlan { get; set; }
    }

    /// <summary>A row in the top physicians or top hospitals list.</summary>
    public class CollectionRowDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        /// <summary>The physician's hospital; empty on the hospital list.</summary>
        public string SecondaryName { get; set; }

        public int Cases { get; set; }
        public int Units { get; set; }
        public decimal Collection { get; set; }

        /// <summary>Share of Total Sold, for the inline bar.</summary>
        public decimal SharePercent { get; set; }
    }

    public class DailyRevenueRowDto
    {
        public DateTime Date { get; set; }
        public int Cases { get; set; }
        public int Units { get; set; }
        public decimal Revenue { get; set; }

        /// <summary>Share of the busiest day in the range, for the inline bar.</summary>
        public decimal SharePercent { get; set; }
    }

    public class RevenueDashboardInput
    {
        /// <summary>Optional. Defaults to the first day of the current month.</summary>
        public DateTime? FromDate { get; set; }

        /// <summary>Optional. Defaults to today.</summary>
        public DateTime? ToDate { get; set; }

        public int? HospitalId { get; set; }
    }
}

namespace ATI.Revenue.Application.Reports.Dtos
{
    /// <summary>
    /// Cases by Person: Cases per physician for last year
    /// </summary>
    public class CasesByPersonReportDto
    {
        public string PhysicianName { get; set; }
        public string HospitalName { get; set; }
        public int TotalCases { get; set; }
        public int DeNovoCases { get; set; }
        public int GenChangeCases { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class CasesByPersonReportInput
    {
        /// <summary>Optional. Defaults to the current year.</summary>
        public int? Year { get; set; }
        public int? HospitalId { get; set; }
        public int? PhysicianId { get; set; }
    }
}

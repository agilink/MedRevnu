namespace ATI.Web.Areas.Core.Models.HostDashboard
{
    public class HostDashboardViewModel
    {
        public int ReportOnLoadDayCount { get; set; } 

        public HostDashboardViewModel(int reportOnLoadDayCount)
        {
            ReportOnLoadDayCount = reportOnLoadDayCount;
        }
    }
}
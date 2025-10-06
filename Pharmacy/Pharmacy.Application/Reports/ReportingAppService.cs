using AspNetCore.Reporting;
using Microsoft.AspNetCore.Hosting;
using DataSet = System.Data.DataSet;

namespace ATI.Pharmacy.Application.Reports
{
    public partial class ReportingAppService : ATIAppServiceBase, IReportingAppService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ReportingAppService(IWebHostEnvironment webHostEnvironment) {
            _webHostEnvironment = webHostEnvironment;
        }
        public MemoryStream GenerateReport(int id)
        {
            LocalReport localReport = new LocalReport(_webHostEnvironment.WebRootPath + "Reports/proarts.rdlc");
           
            System.Data.DataSet dataset = new DataSet();

            localReport.AddDataSource("DataSetName", dataset.Tables[0]);
        
            // Add any parameters if needed
            ReportParameter[] parameters = new ReportParameter[]
            {
                  new ReportParameter("PrescriptionId", id.ToString())
            };
            localReport.SetParameters(parameters);


            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            Microsoft.Reporting.NETCore.Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = localReport.Render(
                reportType,
                null,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return new MemoryStream(renderedBytes);
        }


    }
}

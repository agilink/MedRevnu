using System.Collections.Generic;
using Abp.Collections.Extensions;

using ATI.Pharmacy.Importing.Dto;
using ATI.DataExporting.Excel.MiniExcel;
using ATI.DataImporting.Excel;
using ATI.Dto;
using ATI.Storage;

namespace ATI.Pharmacy;

public class InvalidDoctorExporter(ITempFileCacheManager tempFileCacheManager)
    : MiniExcelExcelExporterBase(tempFileCacheManager), IExcelInvalidEntityExporter<ImportDoctorDto>
{
    public FileDto ExportToFile(List<ImportDoctorDto> doctorList)
{
    var items = new List<Dictionary<string, object>>();

    foreach (var doctor in doctorList)
    {
        items.Add(new Dictionary<string, object>()
            {
                {"Refuse Reason", doctor.Exception},
                    {"DoctorID", doctor.DoctorID},
                    {"Specialty", doctor.Specialty},
                    {"LicenseNumber", doctor.LicenseNumber}
            });
    }

    return CreateExcelPackage("InvalidDoctorImportList.xlsx", items);
}
}
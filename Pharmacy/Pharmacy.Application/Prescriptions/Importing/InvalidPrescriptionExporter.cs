using System.Collections.Generic;
using Abp.Collections.Extensions;

using ATI.Pharmacy.Importing.Dto;
using ATI.DataExporting.Excel.MiniExcel;
using ATI.DataImporting.Excel;
using ATI.Dto;
using ATI.Storage;

namespace ATI.Pharmacy;

public class InvalidPrescriptionExporter(ITempFileCacheManager tempFileCacheManager)
    : MiniExcelExcelExporterBase(tempFileCacheManager), IExcelInvalidEntityExporter<ImportPrescriptionDto>
{
    public FileDto ExportToFile(List<ImportPrescriptionDto> PrescriptionList)
{
    var items = new List<Dictionary<string, object>>();

    foreach (var Prescription in PrescriptionList)
    {
        items.Add(new Dictionary<string, object>()
            {
                {"Refuse Reason", Prescription.Exception},
                    {"PrescriptionID", Prescription.PrescriptionID},
                    {"Specialty", Prescription.Specialty},
                    {"LicenseNumber", Prescription.LicenseNumber}
            });
    }

    return CreateExcelPackage("InvalidPrescriptionImportList.xlsx", items);
}
}
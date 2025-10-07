using System.Collections.Generic;
using Abp.Collections.Extensions;

using ATI.Pharmacy.Importing.Dto;
using ATI.DataExporting.Excel.MiniExcel;
using ATI.DataImporting.Excel;
using ATI.Dto;
using ATI.Storage;

namespace ATI.Pharmacy;

public class InvalidPatientExporter(ITempFileCacheManager tempFileCacheManager)
    : MiniExcelExcelExporterBase(tempFileCacheManager), IExcelInvalidEntityExporter<ImportPatientDto>
{
    public FileDto ExportToFile(List<ImportPatientDto> patientList)
{
    var items = new List<Dictionary<string, object>>();

    foreach (var patient in patientList)
    {
        items.Add(new Dictionary<string, object>()
            {
                {"Refuse Reason", patient.Exception},
                    {"Name", patient.Name},
                    {"Surname", patient.Surname},
                    {"DateOfBirth", patient.DateOfBirth},
                    {"EmergencyContanctName", patient.EmergencyContanctName},
                    {"EmergencyContactPhone", patient.EmergencyContactPhone},
                    {"EmailAddress", patient.EmailAddress},
                    {"PhoneNumber", patient.PhoneNumber},
                    {"FaxNo", patient.FaxNo},
                    {"InsuranceProvider", patient.InsuranceProvider},
                    {"PoliceNumber", patient.PoliceNumber},
                    {"CoverageDetails", patient.CoverageDetails},
                    {"Gender", patient.Gender}
            });
    }

    return CreateExcelPackage("InvalidPatientImportList.xlsx", items);
}
}
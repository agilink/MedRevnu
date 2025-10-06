using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Abp.Localization;
using Abp.Localization.Sources;
using System.Linq;

using Abp.Collections.Extensions;
using ATI.DataExporting.Excel.MiniExcel;
using ATI.DataImporting.Excel;
using ATI.Pharmacy.Importing.Dto;

namespace ATI.Pharmacy;

public class PrescriptionListExcelDataReader(ILocalizationManager localizationManager)
    : MiniExcelExcelImporterBase<ImportPrescriptionDto>(localizationManager), IExcelDataReader<ImportPrescriptionDto>
{

        public List<ImportPrescriptionDto> GetEntitiesFromExcel(byte[] fileBytes)
{
    return ProcessExcelFile(fileBytes, ProcessExcelRow);
}

private ImportPrescriptionDto ProcessExcelRow(dynamic row)
{

    var exceptionMessage = new StringBuilder();
    var Prescription = new ImportPrescriptionDto();

    try
    {
        Prescription.PrescriptionID = Convert.ToInt32(GetRequiredValueFromRowOrNull(row, nameof(Prescription.PrescriptionID), exceptionMessage));
        Prescription.Specialty = GetOptionalValueFromRowOrNull<string>(row, nameof(Prescription.Specialty), exceptionMessage);
        Prescription.LicenseNumber = GetOptionalValueFromRowOrNull<string>(row, nameof(Prescription.LicenseNumber), exceptionMessage);

    }
    catch (Exception exception)
    {
        Prescription.Exception = exception.Message;
    }

    return Prescription;
}

}
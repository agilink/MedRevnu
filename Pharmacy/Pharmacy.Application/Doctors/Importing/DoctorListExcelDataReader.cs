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

public class DoctorListExcelDataReader(ILocalizationManager localizationManager)
    : MiniExcelExcelImporterBase<ImportDoctorDto>(localizationManager), IExcelDataReader<ImportDoctorDto>
{

        public List<ImportDoctorDto> GetEntitiesFromExcel(byte[] fileBytes)
{
    return ProcessExcelFile(fileBytes, ProcessExcelRow);
}

private ImportDoctorDto ProcessExcelRow(dynamic row)
{

    var exceptionMessage = new StringBuilder();
    var doctor = new ImportDoctorDto();

    try
    {
        doctor.DoctorID = Convert.ToInt32(GetRequiredValueFromRowOrNull(row, nameof(doctor.DoctorID), exceptionMessage));
        doctor.Specialty = GetOptionalValueFromRowOrNull<string>(row, nameof(doctor.Specialty), exceptionMessage);
        doctor.LicenseNumber = GetOptionalValueFromRowOrNull<string>(row, nameof(doctor.LicenseNumber), exceptionMessage);

    }
    catch (Exception exception)
    {
        doctor.Exception = exception.Message;
    }

    return doctor;
}

}
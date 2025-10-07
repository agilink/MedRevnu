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

public class PatientListExcelDataReader(ILocalizationManager localizationManager)
    : MiniExcelExcelImporterBase<ImportPatientDto>(localizationManager), IExcelDataReader<ImportPatientDto>
{

    public List<ImportPatientDto> GetEntitiesFromExcel(byte[] fileBytes)
    {
        return ProcessExcelFile(fileBytes, ProcessExcelRow);
    }

    private ImportPatientDto ProcessExcelRow(dynamic row)
    {

        var exceptionMessage = new StringBuilder();
        var patient = new ImportPatientDto();

        try
        {
            patient.Name = GetRequiredValueFromRowOrNull(row, nameof(patient.Name), exceptionMessage);
            patient.Surname = GetOptionalValueFromRowOrNull<string>(row, nameof(patient.Surname), exceptionMessage);
            patient.DateOfBirth = Convert.ToDateTime(GetOptionalValueFromRowOrNull<DateTime>(row, nameof(patient.DateOfBirth), exceptionMessage));
            patient.PatientDateofBirth = Convert.ToDateTime(GetOptionalValueFromRowOrNull<DateTime>(row, nameof(patient.PatientDateofBirth), exceptionMessage));
            patient.EmergencyContanctName = GetOptionalValueFromRowOrNull<string>(row, nameof(patient.EmergencyContanctName), exceptionMessage);
            patient.EmergencyContactPhone = GetOptionalValueFromRowOrNull<string>(row, nameof(patient.EmergencyContactPhone), exceptionMessage);
            patient.EmailAddress = GetOptionalValueFromRowOrNull<string>(row, nameof(patient.EmailAddress), exceptionMessage);
            patient.PhoneNumber = GetOptionalValueFromRowOrNull<string>(row, nameof(patient.PhoneNumber), exceptionMessage);
            patient.FaxNo = GetOptionalValueFromRowOrNull<string>(row, nameof(patient.FaxNo), exceptionMessage);
            patient.InsuranceProvider = GetOptionalValueFromRowOrNull<string>(row, nameof(patient.InsuranceProvider), exceptionMessage);
            patient.PoliceNumber = GetOptionalValueFromRowOrNull<string>(row, nameof(patient.PoliceNumber), exceptionMessage);
            patient.CoverageDetails = GetOptionalValueFromRowOrNull<string>(row, nameof(patient.CoverageDetails), exceptionMessage);
            patient.Gender = GetOptionalValueFromRowOrNull<string>(row, nameof(patient.Gender), exceptionMessage);
            patient.PatientAddress = GetOptionalValueFromRowOrNull<string>(row, nameof(patient.PatientAddress), exceptionMessage);
            patient.PatientCity = GetOptionalValueFromRowOrNull<string>(row, nameof(patient.PatientCity), exceptionMessage);
            patient.PatientState = GetOptionalValueFromRowOrNull<string>(row, nameof(patient.PatientState), exceptionMessage);
            patient.PatientZip = GetOptionalValueFromRowOrNull<string>(row, nameof(patient.PatientZip), exceptionMessage);
        }
        catch (Exception exception)
        {
            patient.Exception = exception.Message;
        }

        return patient;
    }

}
using ATI.DataImporting.Excel;
using ATI.Pharmacy.Application.Patients;

namespace ATI.Pharmacy.Importing.Dto;

//[AutoMapTo(typeof(Patient))]
public class ImportPatientDto : ImportFromExcelDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string EmergencyContanctName { get; set; }
    public string EmergencyContactPhone { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string FaxNo { get; set; }
    public string InsuranceProvider { get; set; }
    public string PoliceNumber { get; set; }
    public string CoverageDetails { get; set; }
    public string Gender { get; set; }
    public string PatientAddress { get; set; }
    public string PatientCity { get; set; }
    public string PatientState { get; set; }
    public string PatientZip { get; set; }
    public DateTime PatientDateofBirth { get; set; }
}
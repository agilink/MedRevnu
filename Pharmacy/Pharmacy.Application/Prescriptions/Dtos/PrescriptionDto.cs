

using ATI.Admin.Domain.Entities;

namespace ATI.Pharmacy.Dtos;

public class PrescriptionDto : Abp.Application.Services.Dto.EntityDto
{
    public int PrescriptionID { get; set; }
    public string PatientName { get; set; }
    public string PatientContact { get; set; }
    public string DoctorName { get; set; }
    public string PharmacyName { get; set; }
    public string DeliveryType { get; set; }
    public int DeliveryTypeId { get; set; }
    public int BillingTo { get; set; }
    public string BillToName { get; set; }
    public string Specialty { get; set; }
    public string LicenseNumber { get; set; }
    public string Drugs { get; set; }
    public string BinaryFileId { get; set; }
    public string PrescriptionStatus { get; set; }
    public int? PrescriptionStatusId { get; set; }
    public string PatientsurName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? PrescriptionDate { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public List<PrescriptionItemDto> PrescriptionItems { get; set; }
    public string? CompanyName { get; set; }
    public int? DosageRouteId { get; set; }

    public string? Notes { get; set; }

    public Facility? PrescriberFacility { get; set; }

    public string? ClinicName { get; set; }
    public string? PrescriberName { get; set; }
    public string? PrescriberOfficePhone { get; set; }
    public string? PrescriberAddress1 { get; set; }
    public string? PrescriberAddress2 { get; set; }
    public string? PrescriberSignatureFileID { get; set; }
    public string? PersonFaxing { get; set; }
    public string? PrescriberNPI { get; set; }
    public string? PrescriberOfficeFax { get; set; }
    public string? PatientAddress { get; set; }
    public string? PatientPhone { get; set; }
    public string? DrugAllergies { get; set; }
    public DateTime? PatientDateOfBirth { get; set; }
}
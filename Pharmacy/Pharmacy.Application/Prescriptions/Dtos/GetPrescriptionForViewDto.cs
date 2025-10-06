namespace ATI.Pharmacy.Dtos;

public class GetPrescriptionForViewDto
{
    public PrescriptionDto Prescription { get; set; }
    public PatientDto Patient { get; set; }
    public DoctorDto Doctor { get; set; }
    public string PersonFaxing { get; set; }
    public string SignatureBytes { get; set; }
    public string Header { get; set; }
}
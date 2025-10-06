

namespace ATI.Pharmacy.Dtos;

public class PrescriptionItemDto : Abp.Application.Services.Dto.EntityDto
{

    public int? TenantId { get; set; }
    public Nullable<int> PrescriptionId { get; set; }
    public Nullable<int> MedicationId { get; set; }
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? RouteOfAdministration { get; set; }
    public string? Duration { get; set; }
    public string? SpecialInstructions { get; set; }
    public string? RefillInformation { get; set; }
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public Nullable<int> RefillsAllowed { get; set; }
    public Nullable<int> RefillsRemaining { get; set; }
    public Nullable<DateTime> StartDateTime { get; set; }
    public Nullable<DateTime> EndDateTime { get; set; }
    public Nullable<int> Quantity { get; set; }
    public MedicationDto? Medication { get; set; }
}
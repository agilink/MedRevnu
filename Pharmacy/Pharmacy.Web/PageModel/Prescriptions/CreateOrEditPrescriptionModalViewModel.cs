using ATI.Pharmacy.Dtos;

using Abp.Extensions;
using ATI.Pharmacy.Web.PageModel.Patients;
using Microsoft.AspNetCore.Mvc.Rendering;
using ATI.Pharmacy.Domain.Entities;
using ATI.Pharmacy.Application.Medications.Dtos;
using ATI.Types;

namespace ATI.Pharmacy.Web.PageModel.Prescriptions
{
    public class CreateOrEditPrescriptionModalViewModel
    {
        public CreateOrEditPrescriptionModalViewModel()
        {
            SelectedMedicineIds = new Dictionary<int?, int?>();
            PrescriberFacilities = new List<SelectListItem>();
        }
        public CreateOrEditPrescriptionDto Prescription { get; set; }
        public CreateOrEditPatientModalViewModel Patient { get; set; }
        public IList<MedicationDto> AllMedicines { get; set; }

        public IList<MedicationDto> NauseaMedications { get; set; }
        public IList<MedicationDto> OtherMedications { get; set; }

        public IList<DosageRouteDto> DosageRoutes { get; set; }

        public bool IsEditMode => Prescription.Id.HasValue;

        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorDefaultFacilityId { get; set; }
        public List<SelectListItem> Doctors { get; set; }
        public List<SelectListItem> DeliveryTypes { get; set; }
        public List<SelectListItem> BillingTypes { get; set; }
        public List<SelectListItem> PrescriberFacilities { get; set; }
        public DeliveryTypeEnum SelectedDeliveryType { get; set; }
        public DeliveryTypeEnum SelectedBillingType { get; set; }
        public Dictionary<int?, int?> SelectedMedicineIds { get; set; }
        public string DoctorDefaultFacilityName { get; set; }
    }
}
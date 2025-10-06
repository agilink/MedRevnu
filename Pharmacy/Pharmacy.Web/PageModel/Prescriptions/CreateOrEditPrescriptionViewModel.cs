using ATI.Pharmacy.Dtos;
namespace ATI.Pharmacy.Web.PageModel.Prescriptions
{
    public class CreateOrEditPrescriptionViewModel
    {
        public CreateOrEditPrescriptionDto Prescription { get; set; }

        public bool IsEditMode => Prescription.Id.HasValue;
    }
}
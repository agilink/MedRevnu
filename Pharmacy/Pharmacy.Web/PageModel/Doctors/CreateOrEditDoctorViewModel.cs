using ATI.Pharmacy.Dtos;
namespace ATI.Pharmacy.Web.PageModel.Doctors
{
    public class CreateOrEditDoctorViewModel
    {
        public CreateOrEditDoctorDto Doctor { get; set; }

        public bool IsEditMode => Doctor.Id.HasValue;
    }
}
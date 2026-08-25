using ATI.Revenue.Application.Hospitals.Dtos;

namespace ATI.Revenue.Web.PageModel.Hospitals
{
    public class CreateOrEditHospitalModalViewModel
    {
        public CreateOrEditHospitalDto Hospital { get; set; }
        public bool IsEditMode { get; set; }
    }
}

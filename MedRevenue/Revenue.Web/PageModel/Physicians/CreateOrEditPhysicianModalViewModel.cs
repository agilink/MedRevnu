using ATI.Revenue.Application.Physicians.Dtos;

namespace ATI.Revenue.Web.PageModel.Physicians
{
    public class CreateOrEditPhysicianModalViewModel
    {
        public CreateOrEditPhysicianDto Physician { get; set; }
        public bool IsEditMode { get; set; }
    }
}

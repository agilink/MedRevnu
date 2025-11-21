using ATI.Revenue.Application.Cases.Dtos;

namespace ATI.Revenue.Web.PageModel.Cases
{
    public class CreateOrEditCaseModalViewModel
    {
        public CreateOrEditCaseDto? Case { get; set; }
        public bool IsEditMode { get; set; }
    }
}

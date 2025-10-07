using Microsoft.AspNetCore.Mvc.Rendering;

namespace ATI.Pharmacy.Web.PageModel.Doctors;

public class DoctorsViewModel
{
    public DoctorsViewModel()
    {
        FacilityList = new List<SelectListItem>();
        CompanyList = new List<SelectListItem>();
    }
    public string FilterText { get; set; }
    public List<SelectListItem> FacilityList { get; set; }
    public List<SelectListItem> CompanyList { get; set; }
    public int? DefaultFacilityId { get; set; }
    public int? CompanyId { get; set; }

}
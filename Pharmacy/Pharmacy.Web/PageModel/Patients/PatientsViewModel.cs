using Microsoft.AspNetCore.Mvc.Rendering;

namespace ATI.Pharmacy.Web.PageModel.Patients;

public class PatientsViewModel
{
    public PatientsViewModel()
    {
        FacilityList = new List<SelectListItem>();
    }
    public string FilterText { get; set; }
    public List<SelectListItem> FacilityList { get; set; }
    public int? DefaultFacilityId { get; set; }

}
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ATI.Pharmacy.Web.PageModel.Prescriptions;

public class PharmacyUserViewModel
{
    public PharmacyUserViewModel()
    {
        Company = new List<SelectListItem>();
        Facility = new List<SelectListItem>();
    }
    public string FilterText { get; set; }
    public List<SelectListItem> Company { get; set; }
    public List<SelectListItem> Facility { get; set; }
    public List<SelectListItem> Role { get; set; }

}
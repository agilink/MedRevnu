using GraphQL.Types;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ATI.Pharmacy.Web.PageModel.Prescriptions;

public class PrescriptionsViewModel
{
    public PrescriptionsViewModel()
    {
        PrescriptionStatus = new List<SelectListItem>();
        FacilityList = new List<SelectListItem>();
    }
    public string FilterText { get; set; }
    public List<SelectListItem> PrescriptionStatus { get; set; }
    public List<SelectListItem> FacilityList { get; set; }
    public int? DefaultFacilityId { get; set; }

}
using ATI.Pharmacy.Dtos;

using Abp.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using ATI.Admin.Application.Addresses.Dtos;

namespace ATI.Pharmacy.Web.PageModel.Patients;

public class CreateOrEditPatientModalViewModel
{
    public CreateOrEditPatientModalViewModel()
    {
        Doctors = new List<SelectListItem>();
    }
    public CreateOrEditPatientDto Patient { get; set; }
    public AddressDto Address { get; set; }

    public bool IsEditMode => Patient.Id.HasValue;
    public int HideDelete
    {
        get
        {
            if (Patient.Prescriptions.Count() > 0)
                return 1;
            return 0;
        }
        set { }
    }
    public bool MultipleDoctorAvailable { get; set; }
    public List<SelectListItem> Doctors { get; internal set; }
    public int? DoctorId { get; set; }
}
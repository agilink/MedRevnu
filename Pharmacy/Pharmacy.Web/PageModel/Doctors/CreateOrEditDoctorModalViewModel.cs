using ATI.Admin.Application.Addresses.Dtos;
using ATI.Admin.Application.Companies.Dtos;
using ATI.Admin.Application.Facilities.Dtos;
using ATI.Pharmacy.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ATI.Pharmacy.Web.PageModel.Doctors
{
    public class CreateOrEditDoctorModalViewModel
    {
        public CreateOrEditDoctorDto Doctor { get; set; }
        public CreateOrEditUserDto User { get; set; }
        public CompanyDto Company { get; set; }
        public FacilityDto Facility { get; set; }
        public int[] FacilityIds { get; set; }

        public bool IsEditMode => Doctor.Id.HasValue;
        public bool IsPharmacyLogin { get; set; }

        public AddressDto Address { get; set; }
        public List<SelectListItem> Facilities { get; set; }
        public bool? SignaturePermission { get;  set; }
    }
}
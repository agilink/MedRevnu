
using ATI.Admin.Application.Addresses.Dtos;
using ATI.Pharmacy.Application;

namespace ATI.Pharmacy.Dtos;

public class DoctorDto : Abp.Application.Services.Dto.EntityDto
{
    public int DoctorID { get; set; }

    public string Specialty { get; set; }

    public string LicenseNumber { get; set; }
    public string FaxNumber { get; set; }
    public string FullName { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string SignatureFileID { get; set; }
    public string LastName { get; set; }
    public UserDto User { get; set; }
    public AddressDto Address { get; set; }
}
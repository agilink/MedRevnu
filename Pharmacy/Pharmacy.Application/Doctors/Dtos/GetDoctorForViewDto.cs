using ATI.Pharmacy.Application;

namespace ATI.Pharmacy.Dtos;

public class GetDoctorForViewDto
{
    public DoctorDto Doctor { get; set; }
    public string Facility { get; set; }

}
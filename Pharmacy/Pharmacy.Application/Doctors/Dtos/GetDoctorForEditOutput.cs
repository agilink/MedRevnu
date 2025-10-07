using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using ATI.Admin.Application.Companies.Dtos;
using ATI.Admin.Application.Facilities.Dtos;
using ATI.Admin.Application.Addresses.Dtos;

namespace ATI.Pharmacy.Dtos;

public class GetDoctorForEditOutput
{
    public GetDoctorForEditOutput()
    {
        Address = new AddressDto();
    }
    public CreateOrEditDoctorDto Doctor { get; set; }
    public CreateOrEditUserDto User { get; set; }
    public AddressDto Address { get; set; }
    public CompanyDto Company { get; set; }
    public FacilityDto Facility { get; set; }
    public List<FacilityDto> Facilities { get; set; }
    public string PersonFaxing { get; set; }
    public string BillTo { get; set; }
    public string DeliveryType { get; set; }
}
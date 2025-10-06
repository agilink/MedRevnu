using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using ATI.Admin.Application.Addresses.Dtos;

namespace ATI.Pharmacy.Dtos;

public class GetPatientForEditOutput
{
    public CreateOrEditPatientDto Patient { get; set; }
    public AddressDto Address { get; set; }

}
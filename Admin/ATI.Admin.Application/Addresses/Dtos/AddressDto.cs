using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using ATI.Admin.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ATI.Admin.Application.Addresses.Dtos;
public class AddressDto : EntityDto<int?>
{
    public AddressDto()
    {
        States = new List<SelectListItem>();
    }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public int? StateId { get; set; }
    public string ZipCode { get; set; }
    public string ContactFirstName { get; set; }
    public string ContactLastName { get; set; }
    public string PhoneNo { get; set; }
    public string FaxNo { get; set; }
    public string Email { get; set; }

    public string LabeledAddress => $"{Address1} {City} {ZipCode}";

    public bool IsFaxRequired { get; set; }
    public StateDto State { get; set; }
    public List<SelectListItem> States { get; set; }
}
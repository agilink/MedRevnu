using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using ATI.Admin.Application.Companies.Dtos;
using ATI.Admin.Domain.Entities;
using ATI.Admin.Application.CompanyUser.Dtos;
using ATI.Authorization.Roles.Dto;
using ATI.Authorization.Roles;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ATI.Pharmacy.Dtos;

public class CreateOrEditUserInputDto
{
    public int? UserId { get; set; }

    [Required(ErrorMessage = "User Name is required.")]
    [StringLength(50, ErrorMessage = "User Name cannot exceed 50 characters.")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    public string UserRole { get; set; }
    //[Required(ErrorMessage = "Company is required.")]
    //public string Company { get; set; }
    public string? Facility { get; set; }

    // Initialize RoleList to avoid null reference issues
    public List<SelectListItem>? RoleList { get; set; }
    public List<SelectListItem>? CompanyList { get; set; }
    public List<SelectListItem>? FacilityList { get; set; }
    public int[] FacilityIds { get; set; }

    [Required(ErrorMessage = "First Name is required.")]
    [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Last Name is required.")]
    [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
    public string? Surname { get; set; }

    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string? EmailAddress { get; set; }

    //[Required(ErrorMessage = "Date of Birth is required.")]
    //[DataType(DataType.Date)]
    //public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; }
    public bool IsEditMode { get; set; } = false;
    public bool IsPharmacyLogin { get; set; }
}


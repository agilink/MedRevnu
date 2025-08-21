using Abp.Application.Services.Dto;
using ATI.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace ATI.MedRevnu.Application.LafayetteQuota.Dto
{
    public class PersonnelDto : EntityDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Title { get; set; }
        public string Specialty { get; set; }
        public string LicenseNumber { get; set; }
        public bool IsActive { get; set; }
        public int? CompanyId { get; set; }
        public long? UserId { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUserName { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUserName { get; set; }
    }

    public class CreateOrEditPersonnelDto : EntityDto<int?>
    {
        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(200)]
        public string LastName { get; set; }

        [StringLength(200)]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(200)]
        public string Specialty { get; set; }

        [StringLength(100)]
        public string LicenseNumber { get; set; }

        public bool IsActive { get; set; }

        public int? CompanyId { get; set; }

        public long? UserId { get; set; }

        public CreateOrEditPersonnelDto()
        {
            IsActive = true;
        }
    }

    public class GetPersonnelForViewDto
    {
        public PersonnelDto Personnel { get; set; }
        public string CompanyName { get; set; }
        public string UserName { get; set; }
    }

    public class GetPersonnelForEditOutput
    {
        public CreateOrEditPersonnelDto Personnel { get; set; }
        public string CompanyName { get; set; }
        public string UserName { get; set; }
    }

    public class GetAllPersonnelInput : PagedAndSortedInputDto
    {
        public string? Filter { get; set; }
        public string? FirstNameFilter { get; set; }
        public string? LastNameFilter { get; set; }
        public string? EmailAddressFilter { get; set; }
        public string? TitleFilter { get; set; }
        public string? SpecialtyFilter { get; set; }
        public bool? IsActiveFilter { get; set; }
        public int? CompanyIdFilter { get; set; }
        
        // DataTables properties
        public int Draw { get; set; }
    }

    public class GetAllPersonnelForLookupTableInput : PagedAndSortedInputDto
    {
        public string? Filter { get; set; }
        
        // DataTables properties
        public int Draw { get; set; }
    }

    public class GetAllPersonnelForLookupTableOutput
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
    }
}
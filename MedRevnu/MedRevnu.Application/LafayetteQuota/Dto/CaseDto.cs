using Abp.Application.Services.Dto;
using ATI.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ATI.MedRevnu.Application.LafayetteQuota.Dto
{
    public class CaseDto : EntityDto
    {
        public DateTime Date { get; set; }
        public string CaseNumber { get; set; }
        public string Description { get; set; }
        public string ProcedureType { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public int? DoctorId { get; set; }
        public int? HospitalId { get; set; }
        public int? FacilityId { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ProductCount { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUserName { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUserName { get; set; }
    }

    public class CreateOrEditCaseDto : EntityDto<int?>
    {
        [Required]
        public DateTime Date { get; set; }

        [StringLength(200)]
        public string CaseNumber { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(200)]
        public string ProcedureType { get; set; }

        [StringLength(100)]
        public string Status { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        public int? DoctorId { get; set; }

        public int? HospitalId { get; set; }

        public int? FacilityId { get; set; }

        public List<CreateOrEditCaseProductDto> CaseProducts { get; set; }

        public CreateOrEditCaseDto()
        {
            Date = DateTime.Now;
            Status = "Active";
            CaseProducts = new List<CreateOrEditCaseProductDto>();
        }
    }

    public class GetCaseForViewDto
    {
        public CaseDto Case { get; set; }
        public string DoctorName { get; set; }
        public string HospitalName { get; set; }
        public string FacilityName { get; set; }
        public List<CaseProductDto> CaseProducts { get; set; }
    }

    public class GetCaseForEditOutput
    {
        public CreateOrEditCaseDto Case { get; set; }
        public string DoctorName { get; set; }
        public string HospitalName { get; set; }
        public string FacilityName { get; set; }
    }

    public class GetAllCasesInput : PagedAndSortedInputDto
    {
        public string? Filter { get; set; }
        public string? CaseNumberFilter { get; set; }
        public string? ProcedureTypeFilter { get; set; }
        public string? StatusFilter { get; set; }
        public DateTime? DateFromFilter { get; set; }
        public DateTime? DateToFilter { get; set; }
        public int? DoctorIdFilter { get; set; }
        public int? HospitalIdFilter { get; set; }
        public int? FacilityIdFilter { get; set; }
        public decimal? RevenueFromFilter { get; set; }
        public decimal? RevenueToFilter { get; set; }
    }

    public class GetAllCasesForLookupTableInput : PagedAndSortedInputDto
    {
        public string? Filter { get; set; }
    }

    public class GetAllCasesForLookupTableOutput
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
    }
}
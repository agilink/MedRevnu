using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace ATI.Revenue.Application.Hospitals.Dtos
{
    public class CreateOrEditHospitalDto : EntityDto<int>
    {
        [Required]
        [StringLength(200)]
        public string HospitalName { get; set; }

        public int? FacilityStatusId { get; set; }

    }
}

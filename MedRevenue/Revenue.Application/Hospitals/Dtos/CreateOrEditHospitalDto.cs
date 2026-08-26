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

        /// <summary>
        /// The company that owns this hospital. Every hospital belongs to one, so either
        /// this or <see cref="NewCompanyName"/> must be supplied.
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        /// Creates a company with this name and attaches the hospital to it, for when the
        /// owning company is not on file yet.
        /// </summary>
        [StringLength(200)]
        public string NewCompanyName { get; set; }
    }
}

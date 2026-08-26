using Abp.Application.Services.Dto;

namespace ATI.Revenue.Application.Hospitals.Dtos
{
    public class HospitalDto : EntityDto<int>
    {
        public string HospitalName { get; set; }
        public int? FacilityStatusId { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }

        /// <summary>Physicians currently assigned to this hospital.</summary>
        public int PhysicianCount { get; set; }

        /// <summary>Contracted product prices held for this hospital.</summary>
        public int ProductPriceCount { get; set; }
    }
}

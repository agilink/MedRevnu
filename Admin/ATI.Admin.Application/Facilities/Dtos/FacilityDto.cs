using Abp.Application.Services.Dto;
using ATI.Admin.Application.Addresses.Dtos;
using ATI.Admin.Application.Companies.Dtos;
using ATI.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATI.Admin.Application.Facilities.Dtos
{
    public class FacilityDto : EntityDto
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public AddressDto Address { get; set; }
        public CompanyDto Company { get; set; }

        public int? FacilityStatusId { get; set; }
    }
}

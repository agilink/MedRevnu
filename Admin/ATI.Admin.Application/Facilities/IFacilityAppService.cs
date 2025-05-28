using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Admin.Application.Companies.Dtos;
using ATI.Admin.Application.Facilities.Dtos;
using ATI.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATI.Admin.Application.Facilities
{
    public interface IFacilityAppService : IApplicationService
    {
        Task<List<FacilityDto>> GetAll();

        FacilityDto Get(EntityDto input);

        Task CreateOrEdit(CreateOrEditFacilityDto input);

        Task Delete(EntityDto input);
        Task<int> Insert(Facility facility);
    }
}

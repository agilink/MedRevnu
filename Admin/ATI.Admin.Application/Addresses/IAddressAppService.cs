using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Admin.Application.Companies.Dtos;
using ATI.Admin.Application.Facilities.Dtos;
using ATI.Admin.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATI.Admin.Application
{
    public interface IAddressAppService : IApplicationService
    {
        //Task<PagedResultDto<GetAllFacility>> GetAll(GetFacilityInput input);

        //Task<FacilityDto> Get(EntityDto input);
        Task<Address> GetAddress(EntityDto input);
        //Task CreateOrEdit(CreateOrEditFacilityDto input);
        Task CreateOrEdit(Address input);
        //Task Delete(EntityDto input);


        Task<List<SelectListItem>> SelectAllStates();
        Task<List<SelectListItem>> SelectAllStatesByAbbreviation();
    }
}

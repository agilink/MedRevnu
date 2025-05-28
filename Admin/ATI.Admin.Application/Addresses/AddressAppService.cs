using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using ATI.Admin.Application.Companies;
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
    public class AddressAppService : ATIAppServiceBase, IAddressAppService
    {
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<State> _stateRepository;
        public AddressAppService(IRepository<State> stateRepository, IRepository<Address> addressRepository)
        {
            _stateRepository = stateRepository;
            _addressRepository = addressRepository;
        }
        //public Task CreateOrEdit(CreateOrEditFacilityDto input)
        //{
        //    return null;
        //}

        public virtual async Task CreateOrEdit(Address input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        public Task Delete(EntityDto input)
        {
            throw new NotImplementedException();
        }

        public Task<FacilityDto> Get(EntityDto input)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResultDto<GetAllFacility>> GetAll(GetFacilityInput input)
        {
            throw new NotImplementedException();
        }


        public virtual async Task<List<SelectListItem>> SelectAllStates()
        {
            var States = await _stateRepository.GetAllAsync();
            var result = new List<SelectListItem>();
            result.Add(new SelectListItem() { Text = "Select State", Value = "" });
            result.AddRange(States.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),   // The value of the select list item (typically ID)
                Text = c.Description,
                // The displayed text of the select list item (typically Name)
            }).ToList());

            return result;
        }

        public virtual async Task<List<SelectListItem>> SelectAllStatesByAbbreviation()
        {
            var States = await _stateRepository.GetAllAsync();
            var result = new List<SelectListItem>();
            result.Add(new SelectListItem() { Text = "Select State", Value = "" });
            result.AddRange(States.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),   // The value of the select list item (typically ID)
                Text = c.Abbreviation
                // The displayed text of the select list item (typically Name)
            }).ToList());

            return result;
        }

        public virtual async Task<Address> GetAddress(EntityDto input)
        {
            var address = await _addressRepository.FirstOrDefaultAsync(input.Id);
            return address;
        }


        protected virtual async Task Create(Address input)
        {
            var address = ObjectMapper.Map<Address>(input);

            if (AbpSession.TenantId != null)
            {
                // address.TenantId = (int)AbpSession.TenantId;
            }

            await _addressRepository.InsertAsync(address);
        }

        protected virtual async Task Update(Address input)
        {
            var address = await _addressRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, address);
        }
    }
}

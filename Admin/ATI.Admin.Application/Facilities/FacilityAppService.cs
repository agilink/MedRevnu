using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using ATI.Admin.Application.Addresses.Dtos;
using ATI.Admin.Application.Companies;
using ATI.Admin.Application.Companies.Dtos;
using ATI.Admin.Application.Facilities.Dtos;
using ATI.Admin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ATI.Admin.Application.Facilities
{
    public class FacilityAppService : ATIAppServiceBase, IFacilityAppService
    {
        private readonly IRepository<Facility> _facilityRepo;
        public FacilityAppService(
            IRepository<Facility> facilityRepo
            )
        {
            _facilityRepo = facilityRepo;
        }
        public Task CreateOrEdit(CreateOrEditFacilityDto input)
        {
            throw new NotImplementedException();
        }

        public Task Delete(EntityDto input)
        {
            throw new NotImplementedException();
        }

        public FacilityDto Get(EntityDto input)
        {
            return _facilityRepo.GetAllIncluding(a => a.Address)
                .Where(a => a.Id == input.Id)
            .Select(i => new FacilityDto
            {
                FacilityName = i.FacilityName,
                FacilityStatusId = i.FacilityStatusId,
                FacilityId = i.Id,
                Address = ObjectMapper.Map<AddressDto>(i.Address)
            }).FirstOrDefault();

        }

        public async Task<List<FacilityDto>> GetAll()
        {
            return await _facilityRepo.GetAllIncluding(a => a.Address)
                .Where(i => !i.IsDeleted)
                .Select(i => new FacilityDto
                {
                    FacilityName = i.FacilityName,
                    FacilityStatusId = i.FacilityStatusId,
                    FacilityId = i.Id,
                    Address = ObjectMapper.Map<AddressDto>(i.Address)
                })
                .OrderBy(e => e.FacilityName)
                .ToListAsync();
        }
        public async Task<int> Insert(Facility facility)
        {
            await _facilityRepo.InsertAndGetIdAsync(facility);
            return facility.Id;
        }
    }
}

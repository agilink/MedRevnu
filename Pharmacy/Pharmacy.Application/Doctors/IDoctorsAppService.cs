using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Pharmacy.Dtos;
using ATI.Dto;

using System.Collections.Generic;
using Abp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ATI.Pharmacy.Domain.Entities;

namespace ATI.Pharmacy
{

    public interface IDoctorsAppService : IApplicationService
    {
        Task<PagedResultDto<GetDoctorForViewDto>> GetAll(GetAllDoctorsInput input);

        Task<GetDoctorForViewDto> GetDoctorForView(int id);

        Task<GetDoctorForEditOutput> GetDoctorForEdit(EntityDto input);

        Task<List<string>> GetDoctorExcelColumnsToExcel();

        Task CreateOrEdit(CreateOrEditDoctorDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetDoctorsToExcel(GetAllDoctorsForExcelInput input);
        Task<List<SelectListDto>> GetDoctors(string searchText);
        Task<string> SaveSignature([FromBody] string signature);
        Task<string> GetSignature(string binaryObjectId);

        Task<List<SelectListItem>> GetAllPharmacyDoctor();

        Task<List<SelectListItem>> GetAllDoctor(List<long> userIds);
        Task<Doctor> GetDoctorByUserId(int userId);
        Task SetDefaultFacility(int facilityId);
    }
}
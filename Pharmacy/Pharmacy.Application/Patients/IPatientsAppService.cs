using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Pharmacy.Dtos;
using ATI.Dto;

using System.Collections.Generic;
using Abp;

namespace ATI.Pharmacy;

public interface IPatientsAppService : IApplicationService
{
    Task<PagedResultDto<GetPatientForViewDto>> GetAll(GetAllPatientsInput input);

    Task<GetPatientForViewDto> GetPatientForView(int id);

    Task<GetPatientForEditOutput> GetPatientForEdit(EntityDto input);

    Task<List<string>> GetPatientExcelColumnsToExcel();

    Task<int> CreateOrEdit(CreateOrEditPatientDto input);

    Task Delete(EntityDto input);

    Task<FileDto> GetPatientsToExcel(GetAllPatientsForExcelInput input);
    Task<List<SelectListDto>> GetPatients(string searchText);
    Task<int> SavePatients(CreateOrEditPatientDto createOrEditPatientDto);
}
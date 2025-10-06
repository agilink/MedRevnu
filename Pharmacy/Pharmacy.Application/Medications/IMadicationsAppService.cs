using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Pharmacy.Dtos;
using ATI.Dto;

using System.Collections.Generic;
using Abp;
using ATI.Pharmacy.Application.Medications.Dtos;

namespace ATI.Pharmacy;

public interface IMadicationsAppService : IApplicationService
{
    //Task<PagedResultDto<GetPatientForViewDto>> GetAll(GetAllPatientsInput input);

    //Task<GetPatientForViewDto> GetPatientForView(int id);

    //Task<GetPatientForEditOutput> GetPatientForEdit(EntityDto input);

    //Task<List<string>> GetPatientExcelColumnsToExcel();

    //Task CreateOrEdit(CreateOrEditPatientDto input);

    //Task Delete(EntityDto input);

    //Task<FileDto> GetPatientsToExcel(GetAllPatientsForExcelInput input);
    Task<List<MedicationDto>> GetMedications(int dosageRouteId, int? categoryId);
    Task<List<DosageRouteDto>> GetDosageRoutes();

}
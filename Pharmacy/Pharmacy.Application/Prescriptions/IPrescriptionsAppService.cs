using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Pharmacy.Dtos;
using ATI.Dto;

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace ATI.Pharmacy
{

    public interface IPrescriptionsAppService : IApplicationService
    {
        Task<PagedResultDto<GetPrescriptionForViewDto>> GetAll(GetAllPrescriptionsInput input);

        Task<GetPrescriptionForViewDto> GetPrescriptionForView(int id);

        Task<GetPrescriptionForEditOutput> GetPrescriptionForEdit(EntityDto input);

        Task<List<string>> GetPrescriptionExcelColumnsToExcel();

        Task<int> CreateOrEdit(CreateOrEditPrescriptionDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetPrescriptionsToExcel(GetAllPrescriptionsForExcelInput input);

        Task<int> Submit(EntityDto entity);
        Task BulkSubmitTask(List<int> Ids);
        Task<string> BulkSubmit([FromBody] List<int> Ids);
        Task<string> BulkComplete([FromBody] List<int> Ids);
        int? CopyPrescription(int? id);
    }
}
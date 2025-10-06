using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.Extensions;
using Abp.ObjectMapping;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using ATI.Pharmacy.Importing.Dto;
using ATI.Pharmacy.Dtos;
using ATI.DataImporting.Excel;
using ATI.Notifications;
using ATI.Storage;
using System;
using Abp.UI;
using ATI.Pharmacy.Domain.Entities;

namespace ATI.Pharmacy;

public class ImportDoctorsToExcelJob(
    IObjectMapper objectMapper,
    IUnitOfWorkManager unitOfWorkManager,
    DoctorListExcelDataReader dataReader,
    InvalidDoctorExporter invalidEntityExporter,
    IAppNotifier appNotifier,
    IRepository<Doctor> doctorRepository,

    IBinaryObjectManager binaryObjectManager)
    : ImportToExcelJobBase<ImportDoctorDto, DoctorListExcelDataReader, InvalidDoctorExporter>(appNotifier,
        binaryObjectManager, unitOfWorkManager, dataReader, invalidEntityExporter)
{
    public override string ErrorMessageKey => "FileCantBeConvertedToDoctorList";

public override string SuccessMessageKey => "AllDoctorsSuccessfullyImportedFromExcel";

protected override async Task CreateEntityAsync(ImportDoctorDto entity)
{
    var doctor = objectMapper.Map<Doctor>(entity);

    // Add your custom validation here.

    await doctorRepository.InsertAsync(doctor);
}
}
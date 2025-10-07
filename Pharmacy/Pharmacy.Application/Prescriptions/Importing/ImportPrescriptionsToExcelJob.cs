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

public class ImportPrescriptionsToExcelJob(
    IObjectMapper objectMapper,
    IUnitOfWorkManager unitOfWorkManager,
    PrescriptionListExcelDataReader dataReader,
    InvalidPrescriptionExporter invalidEntityExporter,
    IAppNotifier appNotifier,
    IRepository<Prescription> PrescriptionRepository,

    IBinaryObjectManager binaryObjectManager)
    : ImportToExcelJobBase<ImportPrescriptionDto, PrescriptionListExcelDataReader, InvalidPrescriptionExporter>(appNotifier,
        binaryObjectManager, unitOfWorkManager, dataReader, invalidEntityExporter)
{
    public override string ErrorMessageKey => "FileCantBeConvertedToPrescriptionList";

public override string SuccessMessageKey => "AllPrescriptionsSuccessfullyImportedFromExcel";

protected override async Task CreateEntityAsync(ImportPrescriptionDto entity)
{
    var Prescription = objectMapper.Map<Prescription>(entity);

    // Add your custom validation here.

    await PrescriptionRepository.InsertAsync(Prescription);
}
}
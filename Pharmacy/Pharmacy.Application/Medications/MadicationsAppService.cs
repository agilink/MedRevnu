using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using ATI.Pharmacy.Exporting;
using ATI.Pharmacy.Dtos;
using ATI.Dto;
using Abp.Application.Services.Dto;
using ATI.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using ATI.Storage;
using ATI.Exporting;
using ATI.Pharmacy.Domain.Entities;
using Abp.Domain.Uow;
using ATI.Authorization.Users.Dto;
using ATI.Authorization.Users;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using PayPalCheckoutSdk.Orders;
using System.Net.Mail;
using Abp;
using ATI.Pharmacy.Application.Medications.Dtos;

namespace ATI.Pharmacy;

//[AbpAuthorize(AppPermissions.Pages_Madications)]
public class MadicationsAppService : ATIAppServiceBase, IMadicationsAppService
{
    private readonly IRepository<Medication> _medicineRepository;
    private readonly IRepository<DosageRoute> _dosageRouteRepository;
    public MadicationsAppService(IRepository<Medication> medicineRepository, IRepository<DosageRoute> dosageRouteRepository)
    {
        _medicineRepository = medicineRepository;
        _dosageRouteRepository = dosageRouteRepository;
    }

    public async Task<List<MedicationDto>> GetMedications(int DosageRouteId, int? categoryId)
    {
        var medications = await _medicineRepository.GetAllAsync();

        medications = medications.Where(a => a.DosageRouteId == DosageRouteId && !a.IsDeleted);

        if (categoryId.HasValue)
            medications = medications.Where(a => a.MedicationCategoryId == categoryId);

        var result = new List<MedicationDto>();

        foreach (var medication in medications)
        {
            result.Add(new MedicationDto
            {
                Name = medication.MedicationName,
                MedicineId = medication.Id.ToString(),
                Id = medication.Id,
                Dosage = medication.Dosage,
                Instructions = medication.Instructions,
                Description = medication.Description,
                DosageRouteId = medication.DosageRouteId
            });
        }

        return result.ToList();
    }

    public async Task<List<DosageRouteDto>> GetDosageRoutes()
    {
        var dosageRoutes = await _dosageRouteRepository.GetAllAsync();
        //filter
        dosageRoutes = dosageRoutes.Where(a => a.Active);
        var result = new List<DosageRouteDto>();

        foreach (var dosage in dosageRoutes)
        {
            result.Add(new DosageRouteDto
            {
                Id = dosage.Id,
                Description = dosage.Description
            });
        }

        return result.ToList();
    }
}
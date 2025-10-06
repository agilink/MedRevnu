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
using MimeKit;
using PayPalCheckoutSdk.Orders;
using System.Net.Mail;
using ATI.Admin.Application.Companies;
using ATI.Admin.Application;
using Microsoft.EntityFrameworkCore.Query;
using Abp;
using Microsoft.AspNetCore.Mvc;
using ATI.Pharmacy.Application.Patients;
using AutoMapper.Internal.Mappers;
using ATI.Pharmacy.Application.Alergy;
using GetUsersInput = ATI.Authorization.Users.Dto.GetUsersInput;
using static ATI.Configuration.AppSettings;
using Abp.Domain.Entities;
using Abp.Collections.Extensions;
using System.Numerics;
using ATI.Admin.Application.Addresses.Dtos;

namespace ATI.Pharmacy;

public class PatientsAppService : ATIAppServiceBase, IPatientsAppService
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly IAddressAppService _addressAppService;
    private readonly IPatientsExcelExporter _patientsExcelExporter;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IUserAppService _userAppService;
    private readonly IUserCompanyAppService _userCompanyAppService;
    private readonly UserManager _userManager;
    private readonly IDoctorsAppService _doctorsAppService;
    private readonly IRepository<Allergy> _allergy;
    private readonly IRepository<PatientAllergy> _patientAllergyRepo;

    public PatientsAppService(IRepository<Patient> patientRepository, IAddressAppService addressAppService, IPatientsExcelExporter patientsExcelExporter, IUnitOfWorkManager unitOfWorkManager, IUserAppService userAppService,
        IUserCompanyAppService userCompanyAppService, UserManager userManager, IRepository<Allergy> allergy, IRepository<PatientAllergy> patientAllergyRepo,
        IDoctorsAppService doctorsAppService)
    {
        _patientRepository = patientRepository;
        _addressAppService = addressAppService;
        _patientsExcelExporter = patientsExcelExporter;
        _userAppService = userAppService;
        _unitOfWorkManager = unitOfWorkManager;
        _userCompanyAppService = userCompanyAppService;
        _userManager = userManager;
        _allergy = allergy;
        _patientAllergyRepo = patientAllergyRepo;
        _doctorsAppService = doctorsAppService;
    }

    public virtual async Task<PagedResultDto<GetPatientForViewDto>> GetAll(GetAllPatientsInput input)
    {
        //if there is a comma or space between strings
        char[] delimiters = new char[] { ',', ' ' };
        string[] filterArr = input.Filter?.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim().ToLower()).ToArray() ?? new string[0];

        var filteredPatients = _patientRepository.GetAll().Include(i => i.User);
        //List<long> companyUserList = null;
        var roles = _userManager.GetRolesAsync(_userManager.GetUserById(AbpSession.UserId.Value)).Result;
        if (roles.Contains("0576fe6677554bdb9bbabdd327a3631d") || roles.Contains("User"))
        {
            var loggedInUserFacilities = _userCompanyAppService.GetUserCompany(AbpSession.UserId ?? 0);
            var defaultFacilityId = loggedInUserFacilities?.FacilityId ?? 0;
            
            var usersFor = _userCompanyAppService.GetUsersByFacility(defaultFacilityId);
            filteredPatients = _patientRepository.GetAllIncluding(a => a.Prescriptions).Where(i => usersFor.Contains((long)i.UserId)).Include(i => i.User);
            var doctor = await _doctorsAppService.GetDoctorByUserId((int)AbpSession.UserId);
            //if (doctor != null)
            //    filteredPatients = filteredPatients.Where(a => a.Prescriptions.Any(a => a.DoctorId == doctor.Id) || doctor.Id == a.DoctorId).Include(a => a.User);


        }
        var filteredList = filteredPatients
            .WhereIf(filterArr.Any(), e => filterArr.All(filter =>
            e.User.Name.ToLower().Contains(filter)
        || e.User.Surname.ToLower().Contains(filter)
        || (e.User.PhoneNumber != null && e.User.PhoneNumber.ToLower().Contains(filter))
        || (e.DateOfBirth.HasValue && (e.DateOfBirth.Value.ToString().ToLower().Contains(filter)
        || e.DateOfBirth.Value.Year.ToString().ToLower().Contains(filter)
        || e.DateOfBirth.Value.Month.ToString().ToLower().Contains(filter)
        || e.DateOfBirth.Value.Day.ToString().ToLower().Contains(filter))
        )));

        var pagedAndFilteredPatients = filteredList
            .OrderBy(input.Sorting ?? "id asc")
            .PageBy(input);

        var patients = from o in pagedAndFilteredPatients
                       select new
                       {
                           o.User.Name,
                           o.User.Surname,
                           o.User.PhoneNumber,
                           o.GenderId,
                           o.DateOfBirth,
                           o.Id
                       };

        var totalCount = await filteredPatients.CountAsync();

        var dbList = await patients.ToListAsync();
        var results = new List<GetPatientForViewDto>();

        foreach (var o in dbList)
        {
            var res = new GetPatientForViewDto()
            {
                Patient = new PatientDto
                {

                    Name = o.Name,
                    Surname = o.Surname,
                    PhoneNumber = o.PhoneNumber,
                    GenderName = o.GenderId == null ? "" : ((GenderEnum)o.GenderId).ToString(),
                    DateOfBirth = o.DateOfBirth,
                    Id = o.Id,
                }
            };

            results.Add(res);
        }

        return new PagedResultDto<GetPatientForViewDto>(
            totalCount,
            results
        );

    }

    public virtual async Task<GetPatientForViewDto> GetPatientForView(int id)
    {
        var patient = await _patientRepository.GetAsync(id);
        var output = new GetPatientForViewDto { Patient = ObjectMapper.Map<PatientDto>(patient) };
        return output;
    }

    [AbpAuthorize(AppPermissions.Pages_Patients_Edit)]
    public virtual async Task<GetPatientForEditOutput> GetPatientForEdit(EntityDto input)
    {
        var patients = await _patientRepository.GetAllIncludingAsync(a => a.User, b => b.Insurance, c => c.Address, x => x.PatientAllergies, k => k.Prescriptions);
        var patient = patients.FirstOrDefault(a => a.Id == input.Id);

        //mapping
        CreateOrEditPatientDto createOrEditPatientDto = new CreateOrEditPatientDto()
        {
            Id = patient.Id,
            DateOfBirth = patient.DateOfBirth ?? DateTime.Today,
            GenderId = patient.GenderId,
            PhoneNumber = patient.User?.PhoneNumber,
            //EmergencyContactName = patient.EmergencyContactName,
            //EmergencyContactPhone = patient.EmergencyContactPhone,
            Name = patient.User?.Name,
            Surname = patient.User?.Surname,
            EmailAddress = (patient.User?.EmailAddress != null && patient.User.EmailAddress.Contains("dummy.com") || patient.User.EmailAddress.Contains("placeholder.com")) ? "" : patient.User?.EmailAddress,
            InsuranceProvider = patient.Insurance?.InsuranceProvider,
            PolicyNumber = patient.Insurance?.PolicyNumber,
            PCN = patient.Insurance?.PCN,
            Bin = patient.Insurance?.Bin,
            Group = patient.Insurance?.Group,
            CoverageDetails = patient.Insurance?.CoverageDetails,
            AllergyId = patient.PatientAllergies?.Where(i => i.AllergyId.HasValue).Select(i => i.AllergyId.Value).ToList(),
            Other = patient.PatientAllergies?.Where(i => !i.AllergyId.HasValue).Select(i => i.Other).FirstOrDefault(),
            Prescriptions = patient.Prescriptions.Select(prescription => ObjectMapper.Map<PrescriptionDto>(prescription)).ToList(),
            DoctorId = patient.DoctorId
        };

        //Address Mapping
        var States = await _addressAppService.SelectAllStates();
        var address = patient.Address ?? new Admin.Domain.Entities.Address();
        AddressDto createOrEditAddressDto = new AddressDto()
        {
            Id = address.Id,
            StateId = address.StateId,
            Address1 = address.Address1,
            City = address.City,
            Address2 = address.Address2,
            ZipCode = address.ZipCode,
            FaxNo = address.FaxNo,
            PhoneNo = address.PhoneNo,
            States = States
        };


        var output = new GetPatientForEditOutput { Patient = createOrEditPatientDto, Address = createOrEditAddressDto };

        return output;
    }

    public virtual async Task<int> CreateOrEdit(CreateOrEditPatientDto input)
    {
        if (input.Id == null || input.Id == 0)
        {
            return await Create(input);
        }
        else
        {
            return await Update(input);
        }
    }

    protected virtual async Task<int> Create(CreateOrEditPatientDto input)
    {
        var guid = Guid.NewGuid();
        var email = input.EmailAddress == "" ? $"{guid}@placeholder.com" : input.EmailAddress;
        using (var uow = _unitOfWorkManager.Begin())
        {
            await _userAppService.CreateOrUpdateUser(new CreateOrUpdateUserInput
            {
                User = new UserEditDto
                {
                    EmailAddress = email,
                    IsActive = true,
                    ShouldChangePasswordOnNextLogin = true,
                    IsTwoFactorEnabled = false,
                    IsLockoutEnabled = false,
                    Name = input.Name,
                    Surname = input.Surname,
                    UserName = guid.ToString(),
                    Password = "Tennis01",
                    PhoneNumber = input.PhoneNumber
                },
                AssignedRoleNames = new string[] { "9ecd5e2ae4ac4ad38ef4f66df061c63d" }
            });
            await uow.CompleteAsync();
        }

        var userList = await _userAppService.GetUsers(new GetUsersInput()
        {
            Filter = email,
            MaxResultCount = 1,
            Sorting = "EmailAddress"
        });
        var user = userList.Items.FirstOrDefault();
        var patient = new Patient();
        if (user != null)
        {
            ObjectMapper.Map(input, patient);
            patient.UserId = user.Id;
            ObjectMapper.Map(input, patient.Address);
            patient.Address.Id = patient.AddressId ?? 0;
            ObjectMapper.Map(input, patient.Insurance);
            patient.Insurance.Id = patient.InsuranceID ?? 0;
            if (AbpSession.TenantId != null)
            {
                patient.TenantId = (int)AbpSession.TenantId;
            }
            //Insert Ids
            if (input.AllergyIds != null && input.AllergyIds.Length > 0)
            {
                foreach (var item in input.AllergyIds.Split(",", StringSplitOptions.None))
                {
                    int id = int.Parse(item);
                    if (patient.PatientAllergies.Where(i => i.AllergyId == id).Count() > 0) continue;
                    var allergy = _allergy.Get(id);
                    var patientAllergy = new PatientAllergy();
                    patientAllergy.Allergy = allergy;
                    patient.AddAllergy(patientAllergy);
                }
            }
            if (input.Other != null)
            {
                var patientAllergy = new PatientAllergy();
                var patiernAlery = patient.PatientAllergies?.Where(i => !i.AllergyId.HasValue && !string.IsNullOrEmpty(i.Other)).FirstOrDefault();
                if (patiernAlery == null) patiernAlery = new PatientAllergy();
                else
                    patient.RemoveAllergy(patiernAlery);
                patientAllergy.Other = input.Other;
                patient.AddAllergy(patientAllergy);
            }
            await _patientRepository.InsertAndGetIdAsync(patient);
            if (input.DoctorId > 0)
            {
                var doctor = await _doctorsAppService.GetDoctorForEdit(new EntityDto { Id = (int)input.DoctorId });
                if (doctor != null)
                {
                    patient.DoctorId = input.DoctorId;
                    _userCompanyAppService.InsertUserCompany(doctor.User.Id, user.Id);
                }

            }
            else
            {
                var doctor = await _doctorsAppService.GetDoctorByUserId((int)AbpSession.UserId.Value);
                if (doctor != null)
                {
                    patient.DoctorId = doctor.Id;
                    _patientRepository.Update(patient);
                }
                _userCompanyAppService.InsertUserCompany(AbpSession.UserId.Value, user.Id);
            }
        }

        return patient.Id;

    }

    [AbpAuthorize(AppPermissions.Pages_Patients_Edit)]
    protected virtual async Task<int> Update(CreateOrEditPatientDto input)
    {
        var patients = await _patientRepository.GetAllIncludingAsync(a => a.User, b => b.Address, c => c.Insurance, d => d.PatientAllergies);
        var patient = await patients.FirstOrDefaultAsync(a => a.Id == (int)input.Id);

        if (patient.DoctorId > 0 && input.DoctorId == null)
        {
            input.DoctorId = patient.DoctorId;
        }

        ObjectMapper.Map(input, patient);

        if (patient.User != null)
        {
            ObjectMapper.Map(input, patient.User);
            patient.User.Id = (long)patient.UserId;
        }
        if (patient.AddressId == null) patient.Address = new Admin.Domain.Entities.Address();
        ObjectMapper.Map(input, patient.Address);
        patient.Address.Id = patient.AddressId ?? 0;

        if (patient.InsuranceID == null) patient.Insurance = new Insurance();
        ObjectMapper.Map(input, patient.Insurance);
        patient.Insurance.Id = patient.InsuranceID ?? 0;


        if (patient.PatientAllergies.Count() > 0)
        {
            var ids = patient.PatientAllergies.Where(i => i.AllergyId > 0 && !input.AllergyIds.Contains(i.AllergyId.ToString())).ToList();
            ids.ForEach(id => patient.PatientAllergies.Remove(id));
        }
        //Insert Ids
        if (input.AllergyIds != null && input.AllergyIds.Length > 0)
        {
            foreach (var item in input.AllergyIds.Split(",", StringSplitOptions.None))
            {
                int id = int.Parse(item);
                if (patient.PatientAllergies.Where(i => i.AllergyId == id).Count() > 0) continue;
                var allergy = _allergy.Get(id);
                var patientAllergy = new PatientAllergy();
                patientAllergy.Allergy = allergy;
                patient.AddAllergy(patientAllergy);
            }
        }
        var patiernAlery = patient.PatientAllergies?.Where(i => !i.AllergyId.HasValue && !string.IsNullOrEmpty(i.Other))?.FirstOrDefault();
        if (input.Other != patiernAlery?.Other)
        {
            var patientAllergy = new PatientAllergy();
            if (patiernAlery == null) patiernAlery = new PatientAllergy();
            else
                patient.RemoveAllergy(patiernAlery); //remove other allergies
            patientAllergy.Other = input.Other;
            patient.AddAllergy(patientAllergy);
        }
        _patientRepository.InsertOrUpdateAndGetId(patient);

        return patient.Id;
    }

    [AbpAuthorize(AppPermissions.Pages_Patients_Delete)]
    public virtual async Task Delete(EntityDto input)
    {
        await _patientRepository.DeleteAsync(input.Id);
    }

    public virtual async Task<FileDto> GetPatientsToExcel(GetAllPatientsForExcelInput input)
    {

        var filteredPatients = (await _patientRepository.GetAllReadonlyAsync())
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false
                    || e.User.Name.Contains(input.Filter) || e.User.Surname.Contains(input.Filter) || e.EmergencyContactName.Contains(input.Filter) || e.EmergencyContactPhone.Contains(input.Filter)
                    || e.User.EmailAddress.Contains(input.Filter) || e.User.PhoneNumber.Contains(input.Filter) || e.Address.FaxNo.Contains(input.Filter) || e.Insurance.InsuranceProvider.Contains(input.Filter) ||
                    e.Insurance.PolicyNumber.Contains(input.Filter) || e.Insurance.CoverageDetails.Contains(input.Filter));

        var query = (from o in filteredPatients
                     select new GetPatientForViewDto()
                     {
                         Patient = new PatientDto
                         {
                             Name = o.User.Name,
                             Surname = o.User.Surname,
                             EmailAddress = o.User.EmailAddress,
                             PhoneNumber = o.User.PhoneNumber,
                             Gender = (o.GenderId == 1) ? GenderEnum.Male : GenderEnum.Female,
                             Id = o.Id
                         }
                     });

        var patientListDtos = await query.ToListAsync();

        return _patientsExcelExporter.ExportToFile(patientListDtos, input.SelectedColumns);
    }

    public async Task<List<string>> GetPatientExcelColumnsToExcel()
    {
        return await Task.FromResult(EntityExportHelper.GetEntityColumnNames<PatientDto>());
    }


    public async Task<List<SelectListDto>> GetPatients(string searchText)
    {

        //if there is a comma or space between strings
        char[] delimiters = new char[] { ',', ' ' };
        string[] filterArr = searchText?.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim().ToLower()).ToArray() ?? new string[0];

        var patients = await _patientRepository.GetAllIncludingAsync(a => a.User);

        var currentUserId = AbpSession.UserId;
        //get roles
        var roles = await _userManager.GetRolesAsync(_userManager.GetUserById(currentUserId ?? 0));

        if (roles.Contains("0576fe6677554bdb9bbabdd327a3631d") || roles.Contains("User"))
        {
            //Its a doctor logic
            var getDoctor = await _doctorsAppService.GetDoctorByUserId((int)currentUserId);
            var userCompanies = _userCompanyAppService.GetUserCompanies(currentUserId);
            var userCompany = userCompanies.FirstOrDefault(a => a.IsDefaultFacility) ?? userCompanies.FirstOrDefault();
            if (userCompany != null)
            {
                var usersOfCompany = _userCompanyAppService.GetUsersByFacility(userCompany.FacilityId);
                patients = patients.Where(a => (usersOfCompany != null && usersOfCompany.Contains(a.UserId ?? 0)) || a.UserId == currentUserId);
            }
            if (getDoctor != null)
                patients = patients.Where(a => a.Prescriptions.Any(a => a.DoctorId == getDoctor.Id) || getDoctor.Id == a.DoctorId);
        }
        var result = patients.Select(a => new SelectListDto() { Id = a.Id, Label = a.User.FullName, Value = a.User.FullName }).ToList();

        if (filterArr.Count() > 0)
        {
            result = patients.Where(a => filterArr.All(filter =>
            a.User.Name.ToLower().Contains(filter)
            || (a.User.PhoneNumber != null && a.User.PhoneNumber.ToLower().Contains(filter))
            || a.User.Surname.ToLower().Contains(filter)
            ))
                .Select(a => new SelectListDto() { Id = a.Id, Label = a.User.FullName, Value = a.User.FullName }).ToList();
        }

        return result;
    }

    public async Task<int> SavePatients(CreateOrEditPatientDto createOrEditPatientDto)
    {
        //Save Patients
        using (var uow = _unitOfWorkManager.Begin())
        {
            var guid = Guid.NewGuid();
            var email = createOrEditPatientDto.EmailAddress == "" ? $"{guid}@placeholder.com" : createOrEditPatientDto.EmailAddress;
            await _userAppService.CreateOrUpdateUser(new CreateOrUpdateUserInput
            {

                User = new UserEditDto
                {
                    EmailAddress = email,
                    IsActive = true,
                    ShouldChangePasswordOnNextLogin = true,
                    IsTwoFactorEnabled = false,
                    IsLockoutEnabled = false,
                    Name = createOrEditPatientDto.Name,
                    Surname = createOrEditPatientDto.Surname,
                    UserName = guid.ToString(),
                    Password = "Tennis01",
                    PhoneNumber = createOrEditPatientDto.PhoneNumber
                },
                AssignedRoleNames = new string[] { "9ecd5e2ae4ac4ad38ef4f66df061c63d" }
            });
            await uow.CompleteAsync();
        }
        var userList = await _userAppService.GetUsers(new GetUsersInput()
        {
            Filter = createOrEditPatientDto.EmailAddress,
            MaxResultCount = 1,
            Sorting = "EmailAddress"
        });
        var user = userList.Items.FirstOrDefault();

        var patient = new Patient();
        if (user != null)
        {
            //patient.EmergencyContactName = createOrEditPatientDto.EmergencyContactName;
            //patient.EmergencyContactPhone = createOrEditPatientDto.EmergencyContactPhone;
            //patient.UserId = user.Id;
            //patient.Insurance = new Insurance();
            //patient.Insurance.InsuranceProvider = createOrEditPatientDto.InsuranceProvider;
            //patient.Insurance.CoverageDetails = createOrEditPatientDto.CoverageDetails;
            //patient.Insurance.TenantId = AbpSession.TenantId != null ? AbpSession.TenantId.Value : null;
            //patient.Insurance.PolicyNumber = createOrEditPatientDto.PolicyNumber;
            ObjectMapper.Map(createOrEditPatientDto, patient);
            patient.UserId = user.Id;
            ObjectMapper.Map(createOrEditPatientDto, patient.Address);
            patient.Address.Id = patient.AddressId ?? 0;
            ObjectMapper.Map(createOrEditPatientDto, patient.Insurance);
            patient.Insurance.Id = patient.InsuranceID ?? 0;
            if (AbpSession.TenantId != null)
            {
                patient.TenantId = (int)AbpSession.TenantId;
            }
            //Insert Ids
            if (createOrEditPatientDto.AllergyIds != null && createOrEditPatientDto.AllergyIds.Length > 0)
            {
                foreach (var item in createOrEditPatientDto.AllergyIds.Split(",", StringSplitOptions.None))
                {
                    int id = int.Parse(item);
                    if (patient.PatientAllergies.Where(i => i.AllergyId == id).Count() > 0) continue;
                    var allergy = _allergy.Get(id);
                    var patientAllergy = new PatientAllergy();
                    patientAllergy.Allergy = allergy;
                    patient.AddAllergy(patientAllergy);
                }
            }
            if (createOrEditPatientDto.Other != null)
            {
                var patientAllergy = new PatientAllergy();
                var patiernAlery = patient.PatientAllergies.Where(i => i.AllergyId == 0 && i.Other != null).FirstOrDefault();
                if (patiernAlery == null) patiernAlery = new PatientAllergy();
                patientAllergy.Other = createOrEditPatientDto.Other;
                patient.AddAllergy(patientAllergy);
            }
            _patientRepository.Insert(patient);
        }

        return patient.Id;
    }
}
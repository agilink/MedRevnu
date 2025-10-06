using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using ATI.Admin.Application;
using ATI.Admin.Application.Facilities;
using ATI.Authorization;
using ATI.Authorization.Users;
using ATI.Configuration;
using ATI.Dto;
using ATI.Exporting;
using ATI.Pharmacy.Domain.Entities;
using ATI.Pharmacy.Dtos;
using ATI.Pharmacy.Exporting;
using ATI.Storage;
using Microsoft.EntityFrameworkCore;
using NUglify.Helpers;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using ATI.Admin.Application.Addresses.Dtos;
using ATI.Admin.Domain.Entities;

namespace ATI.Pharmacy;

[AbpAuthorize(AppPermissions.Pages_Prescriptions)]
public class PrescriptionsAppService : ATIAppServiceBase, IPrescriptionsAppService
{
    private readonly IRepository<Prescription> _prescriptionRepository;
    private readonly IRepository<PrescriptionItem> _prescriptionItemRepository;
    private readonly IRepository<Medication> _medicationRepository;
    private readonly IRepository<Doctor> _doctorRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IPrescriptionsExcelExporter _prescriptionsExcelExporter;
    private readonly IPdfGenerationService _pdfGenerationService;
    private readonly IBinaryObjectManager _binaryObjectManager;
    private readonly IUserAppService _userAppService;
    private readonly IUserCompanyAppService _userCompanyAppService;
    private readonly UserManager _userManager;
    private readonly IFaxService _faxService;
    private readonly IAppConfigurationAccessor _appConfigurationAccessor;
    private readonly IFacilityAppService _facilityAppService;
    private readonly IDoctorsAppService _doctorsAppService;

    public PrescriptionsAppService(IRepository<Prescription> prescriptionRepository,
        IRepository<PrescriptionItem> prescriptionItemRepository,
        IRepository<Medication> medicationRepository,
        IRepository<Doctor> doctorRepository,
        IPrescriptionsExcelExporter prescriptionsExcelExporter,
        IPdfGenerationService pdfGenerationService,
        IBinaryObjectManager binaryObjectManager,
        IUserAppService userAppService,
        IUserCompanyAppService userCompanyAppService,
        UserManager userManager,
        IFaxService faxService,
        IAppConfigurationAccessor appConfigurationAccessor,
        IFacilityAppService facilityAppService,
        IDoctorsAppService doctorsAppService,
        IRepository<Patient> patientRepository)
    {
        _prescriptionRepository = prescriptionRepository;
        _prescriptionsExcelExporter = prescriptionsExcelExporter;
        _pdfGenerationService = pdfGenerationService;
        _binaryObjectManager = binaryObjectManager;
        _medicationRepository = medicationRepository;
        _doctorRepository = doctorRepository;
        _userAppService = userAppService;
        _userCompanyAppService = userCompanyAppService;
        _userManager = userManager;
        _faxService = faxService;
        _appConfigurationAccessor = appConfigurationAccessor;
        _facilityAppService = facilityAppService;
        _prescriptionItemRepository = prescriptionItemRepository;
        _doctorsAppService = doctorsAppService;
        _patientRepository = patientRepository;
    }

    public virtual async Task<PagedResultDto<GetPrescriptionForViewDto>> GetAll(GetAllPrescriptionsInput input)
    {

        //if there is a comma or space between strings
        char[] delimiters = new char[] { ',', ' ' };
        string[] filterArr = input.Filter?.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim().ToLower()).ToArray() ?? new string[0];

        //var filteredPrescriptions = _prescriptionRepository.GetAllIncluding(a => a.Patient, b => b.Doctor, k => k.Doctor.Address, c => c.Patient.User, d => d.Doctor.User, x => x.PrescriptionItems)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || EF.Functions.ILike(e.Patient.User.Name, "%" + input.Filter + "%") || EF.Functions.ILike(e.Patient.User.Surname, "%" + input.Filter + "%")
        //                || EF.Functions.ILike(e.Patient.User.PhoneNumber, "%" + input.Filter + "%") || EF.Functions.ILike(e.Doctor.User.Name, "%" + input.Filter + "%") || EF.Functions.ILike(e.Doctor.User.Surname, "%" + input.Filter + "%"))
        //                .WhereIf(input.PrescriptionStatus.HasValue, e => false || e.PrescriptionStatusId == input.PrescriptionStatus.Value);

        var filteredPrescriptions = _prescriptionRepository.GetAllIncluding(a => a.Patient, b => b.Doctor, c => c.Patient.User, d => d.Doctor.User, x => x.PrescriptionItems)
        .WhereIf(filterArr.Any(), e =>
                        filterArr.All(filter =>
                        e.Patient.User.Name.ToLower().Contains(filter)
                        || e.Patient.User.Surname.ToLower().Contains(filter)
                        || (e.Patient.User.PhoneNumber != null && e.Patient.User.PhoneNumber.ToLower().Contains(filter))
                        || e.Doctor.User.Name.ToLower().Contains(filter)
                        || e.Doctor.User.Surname.ToLower().Contains(filter)))
                        .WhereIf(input.PrescriptionStatus.HasValue, e => false || e.PrescriptionStatusId == input.PrescriptionStatus.Value);



        var currentUserId = AbpSession.UserId;
        //get roles
        var roles = await _userManager.GetRolesAsync(_userManager.GetUserById(currentUserId ?? 0));

        if (roles.Contains("0576fe6677554bdb9bbabdd327a3631d") || roles.Contains("User"))
        {
            //Its a doctor logic
            var userCompanies = _userCompanyAppService.GetUserCompany(currentUserId ?? 0);
            var selectedFacilityId = userCompanies?.FacilityId;
            
            if (selectedFacilityId != null)
            {
                filteredPrescriptions = filteredPrescriptions.WhereIf(selectedFacilityId > 0, a => a.PrescriberFacilityId == selectedFacilityId);
            }

            //if (input.defaultFacility != null)
            //    selectedFacilityId = userCompanies?.Where(a => input.defaultFacility == null || a.FacilityId == input.defaultFacility)?.FirstOrDefault()?.FacilityId ?? 0;

            //If a doctor then only can see their prescription
            //var doctor = await _doctorsAppService.GetDoctorByUserId((int)AbpSession.UserId);
            //if (doctor != null)
            //    filteredPrescriptions = filteredPrescriptions.Where(a => a.DoctorId == doctor.Id);
        }
        else if (roles.Contains("3940adad1759401aab8d8a4b37daec8c") || roles.Contains("292b594325de432ba087f999fb429e36"))
        {
            //Pharmacy Admin and Pharmacy user
            //Exclude Created Status
            filteredPrescriptions = filteredPrescriptions.Where(a => a.PrescriptionStatusId > (int)PrescriptionStatusEnum.Created);
        }

        var pagedAndFilteredPrescriptions = filteredPrescriptions
            .OrderBy(input.Sorting ?? "id desc")
            .PageBy(input);

        var facilities = await _facilityAppService.GetAll();

        var prescriptions = from o in pagedAndFilteredPrescriptions
                            select new
                            {
                                o.Id,
                                o.Patient.User.Name,
                                o.Patient.User.Surname,
                                o.Patient.User.PhoneNumber,
                                o.Patient.DateOfBirth,
                                DoctorName = o.Doctor.User.FullName,
                                o.DeliveryTypeId,
                                o.BillingTo,
                                Drugs = string.Join(", <br/>", o.PrescriptionItems.Select(item => item.Medication.MedicationName.Trim() + (item.Medication.IsDeleted ? " (InActive)" : ""))),
                                o.BinaryFileId,
                                o.PrescriptionStatusId,
                                o.PrescriberFacilityId,
                                o.Notes,
                                o.PrescriptionDate,
                                o.LastModificationTime
                            };

        var results = new List<GetPrescriptionForViewDto>();
        var totalCount = await filteredPrescriptions.CountAsync();

            var prescriptionList = prescriptions.ToList();
            var dbList = from p in prescriptionList
                         join f in facilities on p.PrescriberFacilityId equals f.FacilityId
                         select new
                         {
                             p.Id,
                             p.Name,
                             p.Surname,
                             p.PhoneNumber,
                             p.DateOfBirth,
                             p.DoctorName,
                             p.DeliveryTypeId,
                             p.BillingTo,
                             p.Drugs,
                             p.BinaryFileId,
                             p.PrescriptionStatusId,
                             p.PrescriberFacilityId,
                             f.FacilityName,
                             p.Notes,
                             p.PrescriptionDate,
                             p.LastModificationTime,
                             f.Address.FaxNo
                         };
            foreach (var o in dbList)
            {
                var res = new GetPrescriptionForViewDto()
                {
                    Prescription = new PrescriptionDto
                    {
                        Id = o.Id,
                        PrescriptionID = o.Id,
                        PatientName = o.Name,
                        PatientsurName = o.Surname,
                        PrescriptionStatusId = o.PrescriptionStatusId ?? 0,
                        PatientContact = o.PhoneNumber,
                        DateOfBirth = o.DateOfBirth,
                        DoctorName = o.DoctorName,
                        DeliveryType = o.DeliveryTypeId == null ? "" : L(((DeliveryTypeEnum)o.DeliveryTypeId).ToString()),
                        BillToName = o.BillingTo == null ? "" : ((BillToEnum)o.BillingTo).ToString(),
                        Drugs = o.Drugs,
                        Notes = o.Notes,
                        BinaryFileId = o.BinaryFileId,
                        CompanyName = o.FacilityName == null ? "" : o.FacilityName,
                        PrescriptionDate = o.PrescriptionDate,
                        LastModificationTime = o.LastModificationTime,
                        PrescriptionStatus = o.PrescriptionStatusId == 2 ? "Faxed" : (o.PrescriptionStatusId == 3 ? "Completed" : "Created"),
                    },
                    Doctor = new DoctorDto
                    {
                        FaxNumber = o.FaxNo
                    }
                };

                results.Add(res);
            }
    


        return new PagedResultDto<GetPrescriptionForViewDto>(
            totalCount,
            results
        );

    }

    public virtual async Task<GetPrescriptionForViewDto> GetPrescriptionForView(int id)
    {
        var prescriptions = await _prescriptionRepository
            .GetAllIncludingAsync(a => a.Patient,
            c => c.Patient.User,
            d => d.Patient.Insurance,
            h => h.Patient.Address,
            h => h.Patient.Address.State,
            i => i.PrescriberFacility,
            e => e.Doctor,
            f => f.Doctor.User);
        prescriptions = prescriptions.Include(a => a.PrescriptionItems).ThenInclude(a => a.Medication);//Include Medication
        prescriptions = prescriptions.Include(b => b.Patient.PatientAllergies).ThenInclude(a => a.Allergy);
        var prescription = prescriptions.FirstOrDefault(a => a.Id == id);
        var patient = prescription?.Patient;
        var doctor = prescription?.Doctor;

        //Logged in user
        // var user = await _userAppService.GetUserForEdit(new NullableIdDto<long> { Id = AbpSession.UserId });

        var dosageRouteId = (prescription.PrescriptionItems?.FirstOrDefault()?.Medication?.DosageRouteId ?? 0);
        var output = new GetPrescriptionForViewDto
        {
            PersonFaxing = doctor?.User?.Name + " " + doctor?.User?.Surname,
            Prescription = ObjectMapper.Map<PrescriptionDto>(prescription),
            Patient = ObjectMapper.Map<PatientDto>(patient),
            Doctor = ObjectMapper.Map<DoctorDto>(doctor),
            Header = (dosageRouteId == 6 ? "Sublingual" : dosageRouteId == 8 ? "Injectable" : "")
        };

        if (prescription.PrescriberFacilityId > 0 && output.Doctor != null)
        {
            var defaultFacility = _facilityAppService.Get(new EntityDto { Id = prescription.PrescriberFacilityId.Value });
            if (defaultFacility != null)
                output.Doctor.Address = defaultFacility.Address;
        }


        return output;
    }

    [AbpAuthorize(AppPermissions.Pages_Prescriptions_Edit)]
    public virtual async Task<GetPrescriptionForEditOutput> GetPrescriptionForEdit(EntityDto input)
    {
        var prescriptions = await _prescriptionRepository.GetAllIncludingAsync(a => a.Patient, b => b.Patient.User, c => c.Patient.Insurance);
        prescriptions = prescriptions.Include(a => a.PrescriptionItems).ThenInclude(a => a.Medication);
        var prescription = prescriptions.FirstOrDefault(a => a.Id == input.Id);
        var patient = prescription?.Patient;

        //map
        CreateOrEditPrescriptionDto createOrEditPrescriptionDto = ObjectMapper.Map<CreateOrEditPrescriptionDto>(prescription);
        if (createOrEditPrescriptionDto != null)
        {
            createOrEditPrescriptionDto.DeliveryTypeId = prescription?.DeliveryTypeId ?? 0;
            createOrEditPrescriptionDto.BillingTo = prescription?.BillingTo ?? 0;
        }

        CreateOrEditPatientDto createOrEditPatientDto = new CreateOrEditPatientDto()
        {
            Id = patient?.Id,
            Name = patient?.User?.Name,
            Surname = patient?.User?.Name,
            DateOfBirth = patient?.DateOfBirth,
            EmergencyContactName = patient?.EmergencyContactName,
            EmergencyContactPhone = patient?.EmergencyContactPhone,
            EmailAddress = patient?.User?.EmailAddress,
            PhoneNumber = patient?.User?.PhoneNumber,
            FaxNo = "",
            GenderId = patient?.GenderId,
            InsuranceProvider = patient?.Insurance?.InsuranceProvider,
            PolicyNumber = patient?.Insurance?.PolicyNumber,
            CoverageDetails = patient?.Insurance?.CoverageDetails
        };

        var output = new GetPrescriptionForEditOutput { Prescription = createOrEditPrescriptionDto, Patient = createOrEditPatientDto };

        return output;
    }

    public virtual async Task<int> CreateOrEdit(CreateOrEditPrescriptionDto input)
    {
        if (input.Id == 0)
        {
            return await Create(input);
        }
        else
        {
            return await Update(input);
        }
    }

    public virtual async Task<int> Submit(EntityDto entity)
    {
        var Id = entity.Id;
        var model = await GetPrescriptionForView(Id);

        if (model == null || model.Prescription.PrescriptionStatusId != (int)PrescriptionStatusEnum.Created)
        {
            return 0;
        }
        if (model != null)
            model.SignatureBytes = await _doctorsAppService.GetSignature(model?.Doctor?.SignatureFileID ?? "");

        var pdfContent = await _pdfGenerationService.GeneratePdfFromPartialViewAsync("/Areas/Pharmacy/Views/Prescriptions/_ViewAndPrint.cshtml", "_ViewAndPrint.cshtml", model);
        var binaryObject = new BinaryObject()
        {
            Bytes = pdfContent,
            Description = $"Prescription_{Id}.pdf",
            TenantId = AbpSession.TenantId
        };
        //Download
        using (var memoryStream = new MemoryStream())
        {
            await _binaryObjectManager.SaveAsync(binaryObject);
        }

        var prescription = await _prescriptionRepository.FirstOrDefaultAsync(a => a.Id == Id);
        prescription.BinaryFileId = binaryObject.Id.ToString();
        prescription.PrescriptionDate = DateTime.UtcNow;
        await _prescriptionRepository.UpdateAsync(prescription);


        //Send to twilio
        try
        {
            await _faxService.SendAsync(pdfContent);
            prescription.PrescriptionStatusId = (int)PrescriptionStatusEnum.Faxed; //Faxed
            await _prescriptionRepository.UpdateAsync(prescription);         //Update Prescription
            return prescription.Id;
        }
        catch (Exception ex)
        {
            return 0;
        }

    }
    public virtual async Task BulkSubmitTask(List<int> prescriptionIds)
    {
        foreach (var item in prescriptionIds)
        {
            await Submit(new EntityDto { Id = item });
        }
    }

    [AbpAuthorize(AppPermissions.Pages_Prescriptions_Submit)]
    public virtual async Task<string> BulkSubmit([FromBody] List<int> Ids)
    {
        var message = "";
        if (Ids.Any())
        {
            var prescriptions = await _prescriptionRepository.GetAllAsync();

            var prescriptionIds = prescriptions.Where(a => a.PrescriptionStatusId == 1 && Ids.Any(b => b == a.Id)).Select(a => a.Id).ToList();

            await BulkSubmitTask(prescriptionIds);
            message = "Prescriptions are submitted successfully!";
        }
        else
        {
            message = "Please select prescription";
        }

        return message;
    }


    [AbpAuthorize(AppPermissions.Pages_Prescriptions_Complete)]
    public virtual async Task<string> BulkComplete([FromBody] List<int> Ids)
    {
        var message = "";
        if (Ids.Any())
        {
            var prescriptions = await _prescriptionRepository.GetAllAsync();

            var prescriptionIds = prescriptions.Where(a => a.PrescriptionStatusId == 2 && Ids.Any(b => b == a.Id)).Select(a => a.Id).ToList();

            foreach (var item in prescriptionIds)
            {
                await Complete(new EntityDto { Id = item });
            }
            message = "Prescriptions are completed.";

        }
        else
        {
            message = "Please select prescription";
        }

        return message;
    }

    [AbpAuthorize(AppPermissions.Pages_Prescriptions_Create)]
    protected virtual async Task<int> Create(CreateOrEditPrescriptionDto input)
    {
        var Prescription = ObjectMapper.Map<Prescription>(input);
        var Medicines = await _medicationRepository.GetAllIncludingAsync(b => b.DosageRoute,
            c => c.DosageFrequency);

        Medicines = Medicines.Where(a => input.PrescriptionItems.Select(b => b.MedicationId).Contains(a.Id));

        //Get Doctor Company
        var doctor = await _doctorRepository.GetAllIncluding(a => a.Address, b => b.User, c => c.Address.State).FirstOrDefaultAsync(a => a.Id == input.DoctorID);
        var loggedUserCompany = _userCompanyAppService.GetUserCompany(AbpSession.UserId);
        var doctorUserCompany = _userCompanyAppService.GetUserCompany(doctor.UserId);
        var currentFacility = loggedUserCompany?.Facility ?? doctorUserCompany?.Facility;

        //Maop Prescription
        Prescription.PrescriptionStatusId = (int)PrescriptionStatusEnum.Created;// Created
        Prescription.PrescriberFacilityId = loggedUserCompany?.FacilityId  ?? (doctorUserCompany?.FacilityId ?? 0);

        Prescription.BillingTo = input.BillingTo;
        Prescription.DeliveryTypeId = input.DeliveryTypeId;
        Prescription.DosageRouteId = input.DosageRouteId;
        Prescription.PrescriptionItems = new List<PrescriptionItem>();
        Prescription.CreationTime = DateTime.UtcNow;
        Prescription.LastModificationTime = DateTime.UtcNow;
        //Prescriber
        Prescription.ClinicName = currentFacility?.FacilityName;
        Prescription.PrescriberName = doctor?.User?.FullName;
        if (doctorUserCompany != null)
        {
            Prescription.PrescriberOfficePhone = currentFacility?.Address?.PhoneNo;
            Prescription.PrescriberAddress1 = currentFacility?.Address?.Address1;
            Prescription.PrescriberAddress2 = $"{currentFacility?.Address?.City}, {currentFacility?.Address?.State?.Abbreviation}, {currentFacility?.Address?.ZipCode}";
            Prescription.PrescriberOfficeFax = currentFacility?.Address?.FaxNo;
        }
        Prescription.PersonFaxing = doctor?.User?.FullName;
        Prescription.PrescriberNPI = doctor?.LicenseNumber;
        Prescription.PrescriberSignatureFileID = doctor?.SignatureFileID;
        //Patient
        var patient = _patientRepository.GetAllIncluding(a => a.Address, b => b.User, c => c.Address.State).Include(a => a.PatientAllergies).ThenInclude(x => x.Allergy).FirstOrDefault(a => a.Id == input.PatientID);
        if (patient != null)
        {
            Prescription.PatientName = patient?.User?.FullName;
            Prescription.PatientAddress = $"{patient?.Address?.Address1}, {patient?.Address?.City}, {patient?.Address?.State?.Abbreviation}, {patient?.Address?.ZipCode}";
            Prescription.PatientPhone = patient?.User?.PhoneNumber;
            Prescription.PatientDateOfBirth = patient?.DateOfBirth;
            var allergies = patient?.PatientAllergies.Where(i => i.Allergy != null).Select(item => item.Allergy?.AllergyName);
            var allergyString = string.Join(", ", allergies);
            var allergiesOthers = patient?.PatientAllergies.Where(i => i.Other != null).Select(item => item.Other).Distinct();
            if (allergiesOthers != null && allergiesOthers.Count() > 0) allergyString = allergyString + ", " + string.Join(", ", allergiesOthers);
            Prescription.DrugAllergies = allergyString;
        }
        foreach (var item in input.PrescriptionItems)
        {
            //Create Item Object
            var medicine = Medicines.Where(a => a.Id == item.MedicationId).FirstOrDefault();

            var prescriptionItem = new PrescriptionItem
            {
                MedicationId = item.MedicationId,
                RefillsAllowed = item.RefillsAllowed,
                RouteOfAdministration = medicine?.DosageRoute?.Description,
                Dosage = medicine?.Dosage?.ToString(),
                Frequency = medicine?.DosageFrequency?.Description,
                Description = medicine?.Description,
                Instructions = medicine?.Instructions
            };

            Prescription.PrescriptionItems.Add(prescriptionItem);
        }
        await _prescriptionRepository.InsertAndGetIdAsync(Prescription);


        return Prescription.Id;

    }

    [AbpAuthorize(AppPermissions.Pages_Prescriptions_Edit)]
    protected virtual async Task<int> Update(CreateOrEditPrescriptionDto input)
    {
        var Prescriptions = await _prescriptionRepository.GetAllIncludingAsync(a => a.PrescriptionItems);
        var Prescription = Prescriptions.FirstOrDefault(a => a.Id == (int)input.Id);
        //ObjectMapper.Map(input, Prescription);

        var Medicines = await _medicationRepository.GetAllIncludingAsync(b => b.DosageRoute,
            c => c.DosageFrequency);

        Medicines = Medicines.Where(a => input.PrescriptionItems.Select(b => b.MedicationId).Contains(a.Id));


        //Get Doctor Company
        var doctor = await _doctorRepository.GetAllIncluding(b => b.User).FirstOrDefaultAsync(a => a.Id == input.DoctorID);
        var loggedUserCompany = _userCompanyAppService.GetUserCompany(AbpSession.UserId);
        var doctorUserCompany = _userCompanyAppService.GetUserCompany(doctor.UserId);
        var currentFacility = loggedUserCompany?.Facility ?? doctorUserCompany?.Facility;
        //Maop Prescription
        Prescription.PrescriptionStatusId = (int)PrescriptionStatusEnum.Created;// Created
        Prescription.PrescriberFacilityId = loggedUserCompany?.FacilityId ?? 0;

        Prescription.BillingTo = input.BillingTo;
        Prescription.DeliveryTypeId = input.DeliveryTypeId;
        Prescription.DosageRouteId = input.DosageRouteId;
        Prescription.DoctorId = input.DoctorID;
        Prescription.PatientId = input.PatientID;
        Prescription.LastModificationTime = DateTime.UtcNow;
        //Prescriber
        Prescription.ClinicName = currentFacility?.FacilityName;
        Prescription.PrescriberName = doctor?.User?.FullName;
        if (doctorUserCompany != null)
        {
            Prescription.PrescriberOfficePhone = currentFacility?.Address?.PhoneNo;
            Prescription.PrescriberAddress1 = currentFacility?.Address?.Address1;
            Prescription.PrescriberAddress2 = $"{currentFacility?.Address?.City}, {currentFacility?.Address?.State?.Abbreviation}, {currentFacility?.Address?.ZipCode}";
            Prescription.PrescriberOfficeFax = currentFacility?.Address?.FaxNo;
        }
        Prescription.PersonFaxing = doctor?.User?.FullName;
        Prescription.PrescriberNPI = doctor?.LicenseNumber;
        Prescription.PrescriberSignatureFileID = doctor?.SignatureFileID;
        //Patient
        var patient = _patientRepository.GetAllIncluding(a => a.Address, b => b.User, c => c.Address.State).Include(a => a.PatientAllergies).ThenInclude(x => x.Allergy).FirstOrDefault(a => a.Id == input.PatientID);
        if (patient != null)
        {
            Prescription.PatientName = patient?.User?.FullName;
            Prescription.PatientAddress = $"{patient?.Address?.Address1}, {patient?.Address?.City}, {patient?.Address?.State?.Abbreviation}, {patient?.Address?.ZipCode}";
            Prescription.PatientPhone = patient?.User?.PhoneNumber;
            Prescription.PatientDateOfBirth = patient?.DateOfBirth;
            var allergies = patient?.PatientAllergies.Where(i => i.Allergy != null).Select(item => item.Allergy?.AllergyName);
            var allergyString = string.Join(", ", allergies);
            var allergiesOthers = patient?.PatientAllergies.Where(i => i.Other != null).Select(item => item.Other).Distinct();
            if (allergiesOthers != null && allergiesOthers.Count() > 0) allergyString = allergyString + ", " + string.Join(", ", allergiesOthers);
            Prescription.DrugAllergies = allergyString;

        }
        Prescription.Notes = input.Notes;

        //filter
        var removableMedicines = Prescription.PrescriptionItems.Where(a => !input.PrescriptionItems.Select(b => b.MedicationId).Contains(a.MedicationId)).ToList();

        if (removableMedicines.Count() > 0)
        {
            var items = Prescription.PrescriptionItems.Where(a => removableMedicines.Any(b => b.MedicationId == a.MedicationId)).ToList();
            foreach (var item in items)
            {
                Prescription.RemovePrescrriptionItem(item);
            }
        }

        foreach (var item in input.PrescriptionItems)
        {
            if (!Prescription.PrescriptionItems.Select(a => a.MedicationId).Contains(item.MedicationId))
            {
                //Create Item Object
                var medicine = Medicines.Where(a => a.Id == item.MedicationId).FirstOrDefault();

                var prescriptionItem = new PrescriptionItem
                {
                    MedicationId = item.MedicationId,
                    RefillsAllowed = item.RefillsAllowed,
                    RouteOfAdministration = medicine?.DosageRoute?.Description,
                    Dosage = medicine?.Dosage?.ToString(),
                    Frequency = medicine?.DosageFrequency?.Description,
                    Description = medicine?.Description,
                    Instructions = medicine?.Instructions
                };

                Prescription.AddPrescriptionItem(prescriptionItem);
            }
            else
            {
                Prescription.PrescriptionItems.FirstOrDefault(a => a.MedicationId == item.MedicationId).RefillsAllowed = item.RefillsAllowed;
            }
        }



        await _prescriptionRepository.InsertOrUpdateAndGetIdAsync(Prescription);

        return Prescription.Id;

    }

    [AbpAuthorize(AppPermissions.Pages_Prescriptions_Delete)]
    public virtual async Task Delete(EntityDto input)
    {
        //Delete All Prescription Items 
        var prescriptions = await _prescriptionRepository.GetAllIncludingAsync(a => a.PrescriptionItems);

        var prescription = prescriptions.FirstOrDefault(a => a.Id == input.Id);

        if (prescription == null)
        { return; }

        prescription.PrescriptionItems.ForEach(item => prescription.RemovePrescrriptionItem(item));


        await _prescriptionRepository.UpdateAsync(prescription);
        await _prescriptionRepository.DeleteAsync(prescription);
    }

    [AbpAuthorize(AppPermissions.Pages_Prescriptions_Complete)]
    public virtual async Task Complete(EntityDto input)
    {
        //Complete Prescription
        var prescriptions = await _prescriptionRepository.GetAllAsync();
        var prescription = prescriptions.FirstOrDefault(a => a.Id == input.Id);

        if (prescription == null)
        { return; }
        prescription.PrescriptionStatusId = (int)PrescriptionStatusEnum.Completed;
        await _prescriptionRepository.UpdateAsync(prescription);
    }

    public virtual async Task<FileDto> GetPrescriptionsToExcel(GetAllPrescriptionsForExcelInput input)
    {

        var filteredPrescriptions = (await _prescriptionRepository.GetAllReadonlyAsync())
                    //.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Specialty.Contains(input.Filter) || e.LicenseNumber.Contains(input.Filter))
                    .WhereIf(input.MinPrescriptionIDFilter != null, e => e.Id >= input.MinPrescriptionIDFilter)
                    .WhereIf(input.MaxPrescriptionIDFilter != null, e => e.Id <= input.MaxPrescriptionIDFilter);
        //.WhereIf(!string.IsNullOrWhiteSpace(input.SpecialtyFilter), e => e.Specialty.Contains(input.SpecialtyFilter))
        //.WhereIf(!string.IsNullOrWhiteSpace(input.LicenseNumberFilter), e => e.LicenseNumber.Contains(input.LicenseNumberFilter));

        var query = (from o in filteredPrescriptions
                     select new GetPrescriptionForViewDto()
                     {
                         Prescription = new PrescriptionDto
                         {
                             PrescriptionID = o.Id,
                             //Specialty = o.Specialty,
                             //LicenseNumber = o.LicenseNumber,
                             Id = o.Id
                         }
                     });

        var PrescriptionListDtos = await query.ToListAsync();

        return _prescriptionsExcelExporter.ExportToFile(PrescriptionListDtos, input.SelectedColumns);
    }

    public async Task<List<string>> GetPrescriptionExcelColumnsToExcel()
    {
        return await Task.FromResult(EntityExportHelper.GetEntityColumnNames<PrescriptionDto>());
    }

    public int? CopyPrescription(int? id)
    {
        try
        {
            var oldPrescription = _prescriptionRepository.GetAllIncluding(u => u.PrescriptionItems).AsNoTrackingWithIdentityResolution().FirstOrDefault(o => o.Id == id);
            var newPrescription = ObjectMapper.Map<Prescription>(oldPrescription);
            List<PrescriptionItem> prescriptionItems = new();
            oldPrescription.PrescriptionItems.CopyItemsTo(prescriptionItems);
            newPrescription.Id = 0;
            newPrescription.PrescriptionStatusId = (int)PrescriptionStatusEnum.Created;
            newPrescription.CreatorUserId = AbpSession.UserId;
            newPrescription.CreationTime = DateTime.Now;
            newPrescription.LastModificationTime = null;
            newPrescription.LastModifierUserId = null;
            newPrescription.PrescriptionDate = null;
            // Clear the PrescriptionItems list and re-add new items with correct PrescriptionId
            newPrescription.PrescriptionItems.Clear();
            foreach (var item in prescriptionItems)
            {
                var newItem = ObjectMapper.Map<PrescriptionItem>(item);
                newItem.Id = 0;  // Reset Id to ensure a new record is created
                newItem.PrescriptionId = newPrescription.Id;  // Set to the new Prescription Id
                newPrescription.PrescriptionItems.Add(newItem);
            }
            return _prescriptionRepository.InsertAndGetId(newPrescription);
        }
        catch (Exception ex)
        {
            return 0;
        }
    }
}
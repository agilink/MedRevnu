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
using ATI.Authorization.Users;
using ATI.Pharmacy.Application;
using ATI.Authorization.Users.Dto;
using Abp.Domain.Uow;
using ATI.Admin.Domain.Entities;
using ATI.Admin.Application;
using Abp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Abp.Runtime.Session;
using ATI.Admin.Application.CompanyUser.Dtos;
using Microsoft.AspNetCore.Http;
using ATI.Admin.Application.Companies.Dtos;
using MailKit.Search;
using Abp.Collections.Extensions;
using GetUsersInput = ATI.Authorization.Users.Dto.GetUsersInput;
using ATI.Admin.Application.Companies;
using ATI.Admin.Application.Facilities;
using ATI.Admin.Application.Facilities.Dtos;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using ATI.Admin.Application.Addresses.Dtos;


namespace ATI.Pharmacy;

//[AbpAuthorize(AppPermissions.Pages_Doctors)]
public class DoctorsAppService : ATIAppServiceBase, IDoctorsAppService
{
    private readonly IRepository<Doctor> _doctorRepository;
    private readonly IUserAppService _userAppService;
    private readonly IDoctorsExcelExporter _doctorsExcelExporter;
    private readonly UserManager _userManager;
    private readonly IUnitOfWorkManager _unitOfWork;
    private readonly IUserCompanyAppService _userCompanyAppService;
    protected readonly IBinaryObjectManager _binaryObjectManager;
    protected readonly ICompanyAppService _companyAppService;
    protected readonly IFacilityAppService _facilityAppService;
    public DoctorsAppService(IRepository<Doctor> doctorRepository, IUserAppService userAppService,
        IDoctorsExcelExporter doctorsExcelExporter, UserManager userManager, IUnitOfWorkManager unitOfWork, IUserCompanyAppService userCompanyAppService, IBinaryObjectManager binaryObjectManager,
        ICompanyAppService companyAppService,
        IFacilityAppService facilityAppService)
    {
        _doctorRepository = doctorRepository;
        _doctorsExcelExporter = doctorsExcelExporter;
        _userManager = userManager;
        _userAppService = userAppService;
        _unitOfWork = unitOfWork;
        _userCompanyAppService = userCompanyAppService;
        _binaryObjectManager = binaryObjectManager;
        _companyAppService = companyAppService;
        _facilityAppService = facilityAppService;
    }

    public virtual async Task<PagedResultDto<GetDoctorForViewDto>> GetAll(GetAllDoctorsInput input)
    {
        //if there is a comma or space between strings
        char[] delimiters = new char[] { ',', ' ' };
        string[] filterArr = input.Filter?.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim().ToLower()).ToArray() ?? new string[0];

        var filteredDoctors = _doctorRepository.GetAll().Include(a => a.User)
    .WhereIf(filterArr.Any(), e => filterArr.All(filter =>
        (e.LicenseNumber != null && e.LicenseNumber.ToLower().Contains(filter))
        || e.User.Name.ToLower().Contains(filter)
        || e.User.Surname.ToLower().Contains(filter)
        || (e.User.PhoneNumber != null && e.User.PhoneNumber.ToLower().Contains(filter))
        ));

        var currentUserId = AbpSession.UserId.Value;
        var roles = _userManager.GetRolesAsync(_userManager.GetUserById(currentUserId)).Result;

        if (roles.Contains("0576fe6677554bdb9bbabdd327a3631d") || roles.Contains("User"))
        {
            //Its a doctor logic
            var loggedInUserFacilities = _userCompanyAppService.GetUserCompany(AbpSession.UserId ?? 0);
            var defaultFacilityId = loggedInUserFacilities?.FacilityId;
            var usersFor = _userCompanyAppService.GetUsersByFacility(defaultFacilityId.Value);
            filteredDoctors = filteredDoctors.Where(a => usersFor.Contains((long)a.UserId) || usersFor.Contains((long)a.UserId) || a.UserId == currentUserId);
        }
        else if (input.CompanyId > 0 && (roles.Contains("3940adad1759401aab8d8a4b37daec8c") || roles.Contains("292b594325de432ba087f999fb429e36")))
        {
            var companyId = input.CompanyId;
            var usersFor = _userCompanyAppService.GetUsersByCompany(companyId);
            filteredDoctors = filteredDoctors.Where(a => usersFor.Contains((long)a.UserId) || usersFor.Contains((long)a.UserId) || a.UserId == currentUserId);
        }

        var pagedAndFilteredDoctors = filteredDoctors
            .OrderBy(input.Sorting ?? "id asc")
            .PageBy(input);

        var doctors = from o in pagedAndFilteredDoctors
                      select new
                      {
                          o.Id,
                          o.UserId,
                          o.User.Name,
                          o.User.Surname,
                          o.User.PhoneNumber,
                          o.LicenseNumber,
                      };

        var totalCount = await filteredDoctors.CountAsync();

        var dbList = await doctors.ToListAsync();
        var results = new List<GetDoctorForViewDto>();

        foreach (var o in dbList)
        {
            var userCompanies = _userCompanyAppService.GetUserCompanies(o.UserId);
            var userCompany = userCompanies?.FirstOrDefault(a => a.IsDefaultFacility) ?? userCompanies?.FirstOrDefault();
            var res = new GetDoctorForViewDto()
            {
                Doctor = new DoctorDto
                {

                    DoctorID = o.Id,
                    LicenseNumber = o.LicenseNumber,
                    Id = o.Id,
                    FirstName = o.Name,
                    LastName = o.Surname,
                    PhoneNumber = userCompany?.Facility?.Address?.PhoneNo
                },
                Facility = userCompanies?.Select(a => a.Facility.FacilityName)?.Distinct().JoinAsString(", ")
            };

            results.Add(res);
        }

        return new PagedResultDto<GetDoctorForViewDto>(
            totalCount,
            results
        );

    }

    public virtual async Task<GetDoctorForViewDto> GetDoctorForView(int id)
    {
        var doctors = await _doctorRepository.GetAllIncludingAsync(a => a.User);
        var doctor = doctors.Where(a => a.Id == id).FirstOrDefault();
        var output = new GetDoctorForViewDto { Doctor = ObjectMapper.Map<DoctorDto>(doctor) };

        return output;
    }

    public virtual async Task<GetDoctorForEditOutput> GetDoctorForEdit(EntityDto input)
    {
        if (input.Id == 0)
        {
            return new GetDoctorForEditOutput();
        }
        var doctors = await _doctorRepository.GetAllIncludingAsync(a => a.User, b => b.Address);

        var doctor = doctors.Where(a => a.Id == input.Id).FirstOrDefault();

        //Logged in user
        //  var user = await _userAppService.GetUserForEdit(new NullableIdDto<long> { Id = AbpSession.UserId });

        var userCompanies = _userCompanyAppService.GetUserCompanies(doctor.UserId);

        var userCompany = userCompanies?.FirstOrDefault(a => a.IsDefaultFacility) ?? userCompanies?.FirstOrDefault();

        var output = new GetDoctorForEditOutput
        {
            Doctor = ObjectMapper.Map<CreateOrEditDoctorDto>(doctor),
            User = ObjectMapper.Map<CreateOrEditUserDto>(doctor.User),
            Address = ObjectMapper.Map<AddressDto>(userCompany?.Facility?.Address),
            PersonFaxing = doctor?.User?.Name + " " + doctor?.User?.Surname,
            BillTo = Enum.GetName(typeof(BillToEnum), userCompany?.Company?.BillTo ?? 0),
            DeliveryType = (userCompanies.FirstOrDefault(a => a.IsDefaultFacility)?.Company?.DeliveryTypeId ?? 0) != 0 ? L(Enum.GetName(typeof(DeliveryTypeEnum), userCompanies.FirstOrDefault(a => a.IsDefaultFacility)?.Company?.DeliveryTypeId ?? 0)) : "",
            Facilities = ObjectMapper.Map<List<FacilityDto>>(userCompanies.Select(a => a.Facility))
        };


        return output;
    }

    public virtual async Task CreateOrEdit(CreateOrEditDoctorDto input)
    {
        if (input.Id == null)
        {
            await Create(input);
        }
        else
        {
            await Update(input);
        }
    }

    [AbpAuthorize(AppPermissions.Pages_Doctors_Create)]
    protected virtual async Task Create(CreateOrEditDoctorDto input)
    {
        var email = input.EmailAddress == "" ? $"{input.Name}.{input.Surname}@placeholder.com" : input.EmailAddress;
        var userName = (input.Name.Replace(" ", "") + "." + input.Surname.Replace(" ", ""));
        var doctor = ObjectMapper.Map<Doctor>(input);
        //var user = ObjectMapper.Map<User>(input);

        if (AbpSession.TenantId != null)
        {
            doctor.TenantId = (int?)AbpSession.TenantId;
        }
        UnitOfWorkOptions option = new UnitOfWorkOptions();
        option.Scope = System.Transactions.TransactionScopeOption.RequiresNew;
        option.IsTransactional = true;
        var roles = new[] { "User" }; //Role for Prescriber Admin

        using (var uow = _unitOfWork.Begin(option))
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
                    UserName = input.UserName?.Replace(" ", "") ?? userName,
                    //Password = "Tennis01",
                    Id = input.Id
                },
                AssignedRoleNames = roles,
                SendActivationEmail = true,
                SetRandomPassword = true
            });
            await uow.CompleteAsync();
        }


        bool doctorCreated = false;
        try
        {


            //get user id to update for doctor
            var userList = await _userAppService.GetUsers(new GetUsersInput()
            {
                Filter = email,
                MaxResultCount = 1,
                Sorting = "EmailAddress"
            });
            var user = userList.Items.FirstOrDefault();
            if (user != null)
            {
                //Logged in user company
                var firstFacilityId = input.FacilityIds?.FirstOrDefault();
                var loggedInUserCompany = _userCompanyAppService.GetUserCompany(AbpSession.UserId);
                if ((input.FacilityIds == null || input.FacilityIds.Length == 0) && !string.IsNullOrEmpty(input.FacilityName))
                {

                    var company = new Company
                    {
                        CompanyName = input.FacilityName,
                        CompanyStatusId = 1,
                        BillTo = (int)BillToEnum.Patient,
                        DeliveryTypeId = (int)DeliveryTypeEnum.DeliveryToPatientHome
                    };

                    var companyId = await _companyAppService.Insert(company);

                    var facility = new Facility
                    {
                        FacilityName = input.FacilityName,
                        FacilityStatusId = 1,
                        Company = company,
                    };

                    var facilityId = await _facilityAppService.Insert(facility);

                    firstFacilityId = facilityId;

                    var userCompanyId = _userCompanyAppService.InsertUserCompany(new UserCompany
                    {
                        UserId = user.Id,
                        CompanyId = companyId,
                        FacilityId = facilityId,
                        IsDefaultFacility = true

                    });
                    if (userCompanyId == 0)
                        firstFacilityId = _userCompanyAppService.InsertUserCompany(AbpSession.UserId.Value, user.Id);

                }
                else if (input.FacilityIds != null && input.FacilityIds.Length > 0)
                {
                    foreach (var facilityId in input.FacilityIds)
                    {
                        //Selected from option
                        var company = _userCompanyAppService.GetCompanyByFacility(facilityId);
                        _userCompanyAppService.InsertUserCompany(new UserCompany() { UserId = user.Id, CompanyId = company?.Id ?? 0, FacilityId = facilityId });
                    }
                }
                else if (input.FacilityIds == null || input.FacilityIds.Length == 0)
                    firstFacilityId = _userCompanyAppService.InsertUserCompany(AbpSession.UserId.Value, user.Id);

                doctor.UserId = user.Id;
                doctor.Address = new Address();
                ObjectMapper.Map(input, doctor.Address);

                await _doctorRepository.InsertAndGetIdAsync(doctor);
                doctorCreated = doctor.Id > 0;
            }
        }
        catch (Exception ex)
        {
            doctorCreated = false;
        }
        //Delete user if doctorNotCreated
        if (!doctorCreated && !string.IsNullOrEmpty(email))
        {
            var currentUser = await _userManager.FindByEmailAsync(email);
            if (currentUser != null)
            {
                await _userAppService.DeleteUser(new EntityDto<long> { Id = (long)currentUser.Id });
            }
        }
    }

    [AbpAuthorize(AppPermissions.Pages_Doctors_Edit)]
    protected virtual async Task Update(CreateOrEditDoctorDto input)
    {
        var doctorAll = await _doctorRepository.GetAllIncludingAsync(b => b.Address);
        var doctor = doctorAll.FirstOrDefault(a => a.Id == (int)input.Id);

        if (doctor.SignatureFileID != input.SignatureFileID && !string.IsNullOrEmpty(doctor.SignatureFileID))
        {
            //Delete the old object
            await _binaryObjectManager.DeleteAsync(Guid.Parse(doctor.SignatureFileID));
        }

        ObjectMapper.Map(input, doctor);
        //ObjectMapper.Map(input, doctor.User);
        //doctor.User.Password = "123qwe";
        //doctor.User.UserName = input.UserName ?? (input.Name + '.' + input.Surname);

        //Get Doctor user details
        var doctorUserDetails = await _userAppService.GetUserForEdit(new NullableIdDto<long> { Id = doctor.UserId });
        if (doctorUserDetails?.User != null)
        {
            //Update Doctor user details

            var existingUserName = doctorUserDetails.User.UserName;
            ObjectMapper.Map(input, doctorUserDetails.User);
            doctorUserDetails.User.Id = doctor.UserId;

            //Set User Name
            if (doctorUserDetails.User.UserName == null)
            {
                doctorUserDetails.User.UserName = existingUserName ?? (input.Name + '.' + input.Surname);
            }
            var dataToUpdate = new CreateOrUpdateUserInput
            {
                User = doctorUserDetails.User,
                AssignedRoleNames = new[] { "User" }
            };
            await _userAppService.CreateOrUpdateUser(dataToUpdate);
        }



        //doctor.User.Id = (long)doctor.UserId;

        //Delete existing user company map
        if ((input.FacilityIds != null && input.FacilityIds.Length > 0) || !string.IsNullOrEmpty(input.FacilityName))
        {
            var existingUserCompanies = _userCompanyAppService.GetUserCompanies(doctor.UserId);
            if (existingUserCompanies != null && existingUserCompanies.Count > 0)
            {
                foreach (var existingUserCompany in existingUserCompanies)
                    _userCompanyAppService.DeleteUserCompany(existingUserCompany);
            }
        }

        //Logged in user company
        var loggedInUserCompany = _userCompanyAppService.GetUserCompany(AbpSession.UserId);
        if ((input.FacilityIds == null || input.FacilityIds.Length == 0) && !string.IsNullOrEmpty(input.FacilityName))
        {
            //Create new company

            var company = new Company
            {
                CompanyName = input.FacilityName,
                CompanyStatusId = 1,
                BillTo = (int)BillToEnum.Patient,
                DeliveryTypeId = (int)DeliveryTypeEnum.DeliveryToPatientHome
            };

            var companyId = await _companyAppService.Insert(company);

            var facility = new Facility
            {
                FacilityName = input.FacilityName,
                FacilityStatusId = 1,
                Company = company,
            };

            var facilityId = await _facilityAppService.Insert(facility);
            var userCompanyId = _userCompanyAppService.InsertUserCompany(new UserCompany
            {
                UserId = doctor.UserId,
                CompanyId = companyId,
                FacilityId = facilityId,
                IsDefaultFacility = true

            });
            if (userCompanyId == 0)
                facilityId = _userCompanyAppService.InsertUserCompany(AbpSession.UserId.Value, doctor.UserId);

        }
        else if (input.FacilityIds != null && input.FacilityIds.Length > 0)
        {
            //Selected from option
            foreach (var facilityId in input.FacilityIds)
            {
                var company = _userCompanyAppService.GetCompanyByFacility(facilityId);
                _userCompanyAppService.InsertUserCompany(new UserCompany() { UserId = doctor.UserId, CompanyId = company?.Id ?? 0, FacilityId = facilityId });
            }
        }

        var addressId = 0;
        if (doctor.Address == null) doctor.Address = new Admin.Domain.Entities.Address();
        else addressId = doctor.Address.Id;
        ObjectMapper.Map(input, doctor.Address);
        doctor.Address.Id = addressId;

        //Update Doctor

        await _doctorRepository.UpdateAsync(doctor);
    }

    [AbpAuthorize(AppPermissions.Pages_Doctors_Delete)]
    public virtual async Task Delete(EntityDto input)
    {
        var doctors = await _doctorRepository.GetAllAsync();
        var doctor = doctors.FirstOrDefault(a => a.Id == input.Id);
        await _doctorRepository.DeleteAsync(input.Id);

        //Delete user
        if (doctor != null)
        {
            await _userAppService.DeleteUser(new EntityDto<long> { Id = (long)doctor.UserId });
        }
    }

    public virtual async Task<FileDto> GetDoctorsToExcel(GetAllDoctorsForExcelInput input)
    {

        var filteredDoctors = (await _doctorRepository.GetAllReadonlyAsync())
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Specialty.Contains(input.Filter) || e.LicenseNumber.Contains(input.Filter))
                    .WhereIf(input.MinDoctorIDFilter != null, e => e.Id >= input.MinDoctorIDFilter)
                    .WhereIf(input.MaxDoctorIDFilter != null, e => e.Id <= input.MaxDoctorIDFilter)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SpecialtyFilter), e => e.Specialty.Contains(input.SpecialtyFilter))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.LicenseNumberFilter), e => e.LicenseNumber.Contains(input.LicenseNumberFilter));

        var query = (from o in filteredDoctors
                     select new GetDoctorForViewDto()
                     {
                         Doctor = new DoctorDto
                         {
                             DoctorID = o.Id,
                             Specialty = o.Specialty,
                             LicenseNumber = o.LicenseNumber,
                             Id = o.Id
                         }
                     });

        var doctorListDtos = await query.ToListAsync();

        return _doctorsExcelExporter.ExportToFile(doctorListDtos, input.SelectedColumns);
    }

    public async Task<List<string>> GetDoctorExcelColumnsToExcel()
    {
        return await Task.FromResult(EntityExportHelper.GetEntityColumnNames<DoctorDto>());
    }

    public async Task<List<SelectListDto>> GetDoctors(string searchText)
    {
        //if there is a comma(,) between strings
        string[] filterArr = searchText?.Split(",").Select(a => a.Trim()).ToArray() ?? new string[0];
        var doctors = await _doctorRepository.GetAllIncludingAsync(a => a.User);

        var result = doctors.Select(a => new SelectListDto() { Id = a.Id, Label = a.User.FullName, Value = a.User.FullName }).ToList();


        result = doctors.WhereIf(filterArr.Count() > 0, a => false ||
        filterArr.All(filter => a.User.Name.Contains(filter) || a.User.PhoneNumber.Contains(filter)))
            .Select(a => new SelectListDto() { Id = a.Id, Label = a.User.FullName, Value = a.User.FullName }).ToList();


        return result;
    }

    [HttpPost]
    public async Task<string> SaveSignature([FromBody] string signature)
    {
        // Convert the base64 string to a byte array
        var base64Data = signature.Substring(signature.IndexOf(",") + 1);
        byte[] bytes = Convert.FromBase64String(base64Data);

        var binaryObject = new BinaryObject();
        using (var memoryStream = new MemoryStream())
        {
            binaryObject = new BinaryObject()
            {
                TenantId = AbpSession.TenantId,
                Bytes = bytes,
                Description = "Signature.png"
            };
            await _binaryObjectManager.SaveAsync(binaryObject);
        }

        return binaryObject?.Id.ToString();
    }

    [HttpPost]
    public async Task<string> GetSignature([FromBody] string binaryObjectId)
    {

        var signature = await _binaryObjectManager.GetOrNullAsync(Guid.Parse(binaryObjectId));
        if (signature != null)
        {
            string imageBase64 = Convert.ToBase64String(signature.Bytes);
            return "data:image/png;base64," + imageBase64;
        }
        else
        {
            return string.Empty;
        }

    }

    public virtual async Task<List<SelectListItem>> GetAllPharmacyDoctor()
    {
        var doctors = await _doctorRepository.GetAllIncludingAsync(x => x.User);

        var currentUserId = AbpSession.UserId;
        var roles = await _userManager.GetRolesAsync(_userManager.GetUserById(currentUserId ?? 0));

        if (roles.Contains("0576fe6677554bdb9bbabdd327a3631d") || roles.Contains("User"))
        {
            //Its a doctor logic
            var loggedInUser = _userCompanyAppService.GetUserCompany(currentUserId ?? 0);
            if (loggedInUser != null)
            {
                var usersFor = _userCompanyAppService.GetUsersByFacility(loggedInUser.FacilityId);
                doctors = doctors.Where(a => usersFor.Contains((long)a.UserId) || usersFor.Contains((long)a.UserId) || a.UserId == currentUserId);
            }
        }

        var result = new List<SelectListItem>();
        result.Add(new SelectListItem() { Value = "", Text = "Select a prescriber" });
        result.AddRange(doctors.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),   // The value of the select list item (typically ID)
            Text = c.User.FullName              // The displayed text of the select list item (typically Name)
        }).ToList());

        return result;

    }

    public virtual async Task<List<SelectListItem>> GetAllDoctor(List<long> userIds)
    {
        var doctors = await _doctorRepository.GetAllIncludingAsync(x => x.User);
        doctors = doctors.WhereIf(userIds.Count > 0, e => userIds.Contains((int)e.UserId));
        var result = new List<SelectListItem>();
        result.Add(new SelectListItem() { Value = "", Text = "Select a prescriber" });
        result.AddRange(doctors.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),   // The value of the select list item (typically ID)
            Text = c.User.FullName              // The displayed text of the select list item (typically Name)
        }).ToList());

        return result;
    }

    public virtual async Task<Doctor> GetDoctorByUserId(int userId)
    {
        var query = await _doctorRepository.GetAllAsync();
        var doctor = query.Where(a => a.UserId == userId).FirstOrDefault();
        return doctor;
    }

    public virtual async Task SetDefaultFacility(int facilityId)
    {
        UnitOfWorkOptions option = new UnitOfWorkOptions();
        option.Scope = System.Transactions.TransactionScopeOption.RequiresNew;
        option.IsTransactional = true;
        UserCompany currentOneUserCompany = null;
        using (var uow = _unitOfWork.Begin(option))
        {
            var userCompanies = _userCompanyAppService.GetUserCompanies(AbpSession.UserId);
            foreach (var company in userCompanies.Where(a => a.IsDefaultFacility))
            {
                //Make all default to false before making one default.
                company.IsDefaultFacility = false;
                _userCompanyAppService.UpdateUserCompany(company);
            }
            currentOneUserCompany = userCompanies.FirstOrDefault(a => a.FacilityId == facilityId);
            if (currentOneUserCompany != null)
            {
                currentOneUserCompany.IsDefaultFacility = true;
                _userCompanyAppService.UpdateUserCompany(currentOneUserCompany);
            }
            uow.Complete();
        }

        _userCompanyAppService.ClearRepoCache(currentOneUserCompany);
    }
}
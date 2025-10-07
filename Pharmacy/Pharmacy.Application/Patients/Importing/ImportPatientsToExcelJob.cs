using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using ATI.Admin.Application;
using ATI.Admin.Domain.Entities;
using ATI.Authorization.Roles;
using ATI.Authorization.Users;
using ATI.DataImporting.Excel;
using ATI.Notifications;
using ATI.Pharmacy.Application.Patients;
using ATI.Pharmacy.Domain.Entities;
using ATI.Pharmacy.Importing.Dto;
using ATI.Storage;
using Microsoft.AspNetCore.Identity;

namespace ATI.Pharmacy;

public class ImportPatientsToExcelJob(
    IObjectMapper objectMapper,
    IUnitOfWorkManager unitOfWorkManager,
    PatientListExcelDataReader dataReader,
    InvalidPatientExporter invalidEntityExporter,
    IAppNotifier appNotifier,
    IUserAppService _userAppService,
    IAddressAppService _addressAppService,
    IPasswordHasher<User> passwordHasher,
    IUserCompanyAppService _userCompanyAppService,
    RoleManager roleManager,
    IRepository<Patient> patientRepository,
    IBinaryObjectManager binaryObjectManager)
    : ImportToExcelJobBase<ImportPatientDto, PatientListExcelDataReader, InvalidPatientExporter>(appNotifier,
        binaryObjectManager, unitOfWorkManager, dataReader, invalidEntityExporter)
{
    public override string ErrorMessageKey => "FileCantBeConvertedToPatientList";
    public override string SuccessMessageKey => "AllPatientsSuccessfullyImportedFromExcel";
    public UserManager UserManager { get; set; }

    protected override async Task CreateEntityAsync(ImportPatientDto entity)
    {
        string patientId = Guid.NewGuid().ToString();
        var tenantId = CurrentUnitOfWork.GetTenantId();

        //add user
        UnitOfWorkOptions option = new UnitOfWorkOptions();
        option.Scope = System.Transactions.TransactionScopeOption.RequiresNew;
        option.IsTransactional = true;
        var roles = new[] { "9ecd5e2ae4ac4ad38ef4f66df061c63d" };

        var user = new User
        {
            UserName = patientId,
            EmailAddress = string.IsNullOrEmpty(entity.EmailAddress) ? patientId + "@none.com" : entity.EmailAddress,
            Name = entity.Name,
            PhoneNumber = entity.PhoneNumber,
            IsActive = true,
            ShouldChangePasswordOnNextLogin = false,
            IsTwoFactorEnabled = false,
            IsLockoutEnabled = false,
            Surname = entity.Surname,
            TenantId = tenantId
        };

        var Password = passwordHasher.HashPassword(user, "tennis01");
        user.Password = Password;

        user.Roles = new List<UserRole>();
        var roleList = roleManager.Roles.ToList();
        var role = await roleManager.GetRoleByNameAsync("9ecd5e2ae4ac4ad38ef4f66df061c63d");
        user.Roles.Add(new UserRole(tenantId, user.Id, role.Id));

        (await UserManager.CreateAsync(user)).CheckErrors();

        var userList = await _userAppService.GetUsers(new ATI.Authorization.Users.Dto.GetUsersInput()
        {
            Filter = patientId + "@none.com",
            MaxResultCount = 1,
            Sorting = "EmailAddress"
        });

        var userDetails = userList.Items.FirstOrDefault();

        //add company
        var loggedInUserCompany = _userCompanyAppService.GetUserCompany(entity.SessionUserId);
        if (loggedInUserCompany.Id > 0)
        {
            //Selected from option
            var facility = _userCompanyAppService.GetFacilityByCompany(loggedInUserCompany.CompanyId);
            _userCompanyAppService.InsertUserCompany(new UserCompany() { UserId = user.Id, CompanyId = loggedInUserCompany.CompanyId, FacilityId = facility.Id });
        }
        else if (loggedInUserCompany.Id == 0)
            _userCompanyAppService.InsertUserCompany(entity.SessionUserId, user.Id);

        //fetch state id
        var States = await _addressAppService.SelectAllStatesByAbbreviation();
        var stateId = States.FirstOrDefault(s => s.Text.Equals(entity.PatientState, StringComparison.OrdinalIgnoreCase))?.Value;

        //add patient
        if (user != null)
        {
            var gender = (entity.Gender.Equals("M", StringComparison.OrdinalIgnoreCase) ||
             entity.Gender.Equals("Male", StringComparison.OrdinalIgnoreCase))
            ? GenderEnum.Male
            : (entity.Gender.Equals("F", StringComparison.OrdinalIgnoreCase) ||
               entity.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase))
              ? GenderEnum.Female
              : (GenderEnum?)null;

            var patient = new Patient
            {
                EmergencyContactName = entity.Name + " " + entity.Surname,
                DateOfBirth = Convert.ToDateTime(entity.PatientDateofBirth),
                EmergencyContactPhone = entity.PhoneNumber,
                GenderId = Convert.ToInt32(gender),
                UserId = user.Id,
                TenantId = tenantId,
                Address = new Address
                {
                    ContactFirstName = entity.Name,
                    ContactLastName = entity.Surname,
                    Address1 = entity.PatientAddress,
                    City = entity.PatientCity,
                    ZipCode = entity.PatientZip,
                    StateId = Convert.ToInt32(stateId),
                    CreatorUserId = entity.SessionUserId,
                    LastModificationTime = DateTime.Now
                }
            };

            await patientRepository.InsertAsync(patient);
        }
    }
}
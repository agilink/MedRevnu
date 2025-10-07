using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Zero.Configuration;
using ATI.Admin.Application;
using ATI.Authorization;
using ATI.Authorization.Users;
using ATI.Authorization.Users.Dto;
using ATI.Pharmacy.Dtos;
using ATI.Url;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using ATI.Authorization.Roles;
using Abp;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Abp.Domain.Repositories;
using ATI.Pharmacy.Domain.Entities;
using GetUsersInput = ATI.Pharmacy.Dtos.GetUsersInput;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using AutoMapper;
using ATI.Admin.Domain.Entities;
using Abp.Domain.Uow;
using Abp.UI;
using ATI.Admin.Application.Companies;
using ATI.Admin.Application.Facilities;
using System.Numerics;

namespace ATI.Pharmacy.Application
{
    public partial class UserExtendedAppService : ATIAppServiceBase, IUserExtendedAppService
    {
        public IAppUrlService AppUrlService { get; set; }
        private readonly UserManager _userManager;
        private readonly IUserCompanyAppService _userCompanyAppService;
        private readonly ICompanyAppService _companyAppService;
        private readonly IFacilityAppService _facilityAppService;
        private readonly RoleManager _roleManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserEmailer _userEmailer;
        private readonly IRepository<Doctor> _doctorRepository;
        private readonly IUnitOfWorkManager _unitOfWork;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        public UserExtendedAppService(UserManager userManager, IUserCompanyAppService userCompanyAppService,
            ICompanyAppService companyAppService,
            IFacilityAppService facilityAppService,
            RoleManager roleManager, IPasswordHasher<User> passwordHasher, IUserEmailer userEmailer, IRepository<Doctor> doctorRepository, IUnitOfWorkManager unitOfWork, IRepository<UserRole, long> userRoleRepository)
        {

            AppUrlService = NullAppUrlService.Instance;

            _userManager = userManager;
            _userCompanyAppService = userCompanyAppService;
            _companyAppService = companyAppService;
            _facilityAppService = facilityAppService;
            _roleManager = roleManager;
            _passwordHasher = passwordHasher;
            _userEmailer = userEmailer;
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;
            _userRoleRepository = userRoleRepository;
        }
        public async Task<PagedResultDto<EmployeeListDto>> GetUsers(GetUsersInput input)
        {
            var excludedIds = new int[] { 1, 2, 4 };
            var usersOfCompany = _userCompanyAppService.GetUsersByFacility(input.Facility ?? 0);
            var lower = input.Filter?.ToLower() ?? "";
            var doctorUserIds = _doctorRepository.GetAll().Select(u => u.UserId).ToArray();
            var query = UserManager.Users.Include(a => a.Roles).Where(u => u.Roles.Count() > 0 && u.Roles.Select(a => a.RoleId).All(b => !excludedIds.Contains(b)) && !doctorUserIds.Contains(u.Id))
                .WhereIf(input.Facility.HasValue && input.Facility > 0, e => usersOfCompany.Any(a => a == e.Id))
                .WhereIf(input.Role.HasValue && input.Role > 0, e => e.Roles.Any(r => r.RoleId == input.Role))
                .WhereIf(input.Filter != null, a => (a.Name != null && a.Name.ToLower().Contains(lower))
                || (a.Surname != null && a.Surname.ToLower().Contains(lower))
                || (a.UserName != null && a.UserName.ToLower().Contains(lower))
                || (a.EmailAddress != null && a.EmailAddress.ToLower().Contains(lower)))
                .AsQueryable();

            var userCount = query.Count();

            var users = query.OrderBy(input.Sorting ?? "id asc").PageBy<Authorization.Users.User>(input).ToList();


            var userListDtos = ObjectMapper.Map<List<EmployeeListDto>>(users);

            var userRoles = await _userRoleRepository.GetAll()
                .Select(userRole => userRole).ToListAsync();

            var distinctRoleIds = userRoles.Select(userRole => userRole.RoleId).Distinct();
            var userIds = userListDtos.Select(user => user.Id).Distinct();
            var roleNames = new Dictionary<int, string>();
            var comapnyDetails = new Dictionary<long, string>();
            foreach (var roleId in distinctRoleIds)
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role != null)
                {
                    roleNames[roleId] = role.DisplayName;
                }
            }

            foreach (var userId in userIds)
            {
                var userCompany = _userCompanyAppService.GetUserCompany(userId);
                if (userCompany != null && userCompany.Company != null)
                {
                    comapnyDetails[userId] = userCompany.Company.CompanyName;
                }
            }

            foreach (var userListDto in userListDtos)
            {
                foreach (var userListRoleDto in userListDto.Roles)
                {
                    if (roleNames.ContainsKey(userListRoleDto.RoleId))
                    {
                        userListRoleDto.RoleName = roleNames[userListRoleDto.RoleId];
                    }
                }

                userListDto.Roles = userListDto.Roles.Where(r => r.RoleName != null).OrderBy(r => r.RoleName).ToList();
                if (comapnyDetails.ContainsKey(userListDto.Id))
                    userListDto.Company = comapnyDetails[userListDto.Id];
            }
            return new PagedResultDto<EmployeeListDto>(
                userCount,
                userListDtos
            );
        }
        public virtual async Task<CreateOrEditUserInputDto> GetEmployeeForEdit(EntityDto entityDto)
        {
            var user = UserManager.Users.Include(a => a.Roles)
                .Where(a => a.Id == entityDto.Id).FirstOrDefault();

            var result = new CreateOrEditUserInputDto();


            ObjectMapper.Map(user, result);

            result.UserRole = user?.Roles?.FirstOrDefault()?.RoleId.ToString() ?? "";
            var userCompany = _userCompanyAppService.GetUserCompany(user.Id);
            result.Facility = userCompany?.FacilityId.ToString() ?? "";
            return result;
        }
        public virtual async Task CreateOrEdit(CreateOrEditUserInputDto input)
        {
            if (input.UserId.HasValue)
            {
                await UpdateUserAsync(input);
            }
            else
            {
                await CreateUserAsync(input);
            }
        }
        protected virtual async Task UpdateUserAsync(CreateOrEditUserInputDto input)
        {

            var user = await UserManager.FindByIdAsync(input.UserId.ToString());

            if (user is null)
            {
                throw new AbpException(L("UserNotFound"));
            }

            var isEmailChanged = user.EmailAddress != input.EmailAddress;

            if (isEmailChanged)
            {
                user.IsEmailConfirmed = false;
            }


            //Update user properties
            ObjectMapper.Map(input, user); //Passwords is not mapped (see mapping configuration)

            CheckErrors(await UserManager.UpdateAsync(user));

            ////Delete User Company
            //if (input.Facility != null)
            //{
            //    _userCompanyAppService.DeleteUserUserCompany((int)user.Id);
            //    var employeeUser = await UserManager.FindByEmailAsync(user.EmailAddress);
            //    if (employeeUser != null)
            //    {
            //        var userCompanyId = 0;
            //        var company = _userCompanyAppService.GetCompanyByFacility(int.Parse(input.Facility));
            //        if (input.Facility != null && int.Parse(input.Facility) > 0 && company != null)
            //        {
            //            userCompanyId = _userCompanyAppService.InsertUserCompany(new UserCompany
            //            {
            //                UserId = employeeUser.Id,
            //                CompanyId = company.Id,
            //                FacilityId = int.Parse(input.Facility)

            //            });
            //        }
            //        if (userCompanyId == 0)
            //            _userCompanyAppService.InsertUserCompany(AbpSession.UserId.Value, employeeUser.Id);
            //    }
            //}

            //Delete existing user company map
            if ((input.FacilityIds != null && input.FacilityIds.Length > 0) || !string.IsNullOrEmpty(input.Facility))
            {
                var existingUserCompanies = _userCompanyAppService.GetUserCompanies(input.UserId);
                if (existingUserCompanies != null && existingUserCompanies.Count > 0)
                {
                    foreach (var existingUserCompany in existingUserCompanies)
                        _userCompanyAppService.DeleteUserCompany(existingUserCompany);
                }
            }

            //Logged in user company
            var loggedInUserCompany = _userCompanyAppService.GetUserCompany(AbpSession.UserId);
            if ((input.FacilityIds == null || input.FacilityIds.Length == 0) && !string.IsNullOrEmpty(input.Facility))
            {
                //Create new company
                
                    var company = new Company
                    {
                        CompanyName = input.Facility,
                        CompanyStatusId = 1,
                        BillTo = (int)BillToEnum.Patient,
                        DeliveryTypeId = (int)DeliveryTypeEnum.DeliveryToPatientHome
                    };

                    var companyId = await _companyAppService.Insert(company);

                    var facility = new Facility
                    {
                        FacilityName = input.Facility,
                        FacilityStatusId = 1,
                        Company = company,
                    };

                    var facilityId = await _facilityAppService.Insert(facility);
                    var userCompanyId = _userCompanyAppService.InsertUserCompany(new UserCompany
                    {
                        UserId = input.UserId,
                        CompanyId = companyId,
                        FacilityId = facilityId,
                        IsDefaultFacility = true

                    });
                    if (userCompanyId == 0)
                        facilityId = _userCompanyAppService.InsertUserCompany(AbpSession.UserId.Value, input.UserId);
                
            }
            else if (input.FacilityIds != null && input.FacilityIds.Length > 0)
            {
                //Selected from option
                foreach (var facilityId in input.FacilityIds)
                {
                    var company = _userCompanyAppService.GetCompanyByFacility(facilityId);
                    _userCompanyAppService.InsertUserCompany(new UserCompany() { UserId = input.UserId, CompanyId = company?.Id ?? 0, FacilityId = facilityId });
                }
            }
            else if (input.FacilityIds == null || input.FacilityIds.Length == 0)
                _userCompanyAppService.InsertUserCompany(AbpSession.UserId.Value, input.UserId);

            var roleList = _roleManager.Roles
                .Select(r => new { r.Id, r.Name }).ToList();
            //Update roles
            CheckErrors(await UserManager.SetRolesAsync(user, roleList.Where(a => a.Id.ToString() == input.UserRole).Select(a => a.Name.ToString()).ToArray()));

            //update organization units
            //await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());


        }

        protected virtual async Task CreateUserAsync(CreateOrEditUserInputDto input)
        {

            if (input.UserName != null)
            {
                input.UserName = input.UserName.Replace(" ", "");
            }

            var user = ObjectMapper.Map<User>(input); //Passwords is not mapped (see mapping configuration)
            user.TenantId = AbpSession.TenantId;
            user.IsActive = true;

            var randomPassword = await _userManager.CreateRandomPassword();
            user.Password = _passwordHasher.HashPassword(user, randomPassword);

            //Assign roles
            var roleList = _roleManager.Roles
                .Select(r => new { r.Id, r.DisplayName }).ToList();

            //Send activation email

            user.Roles = new List<UserRole>();

            foreach (var roleName in roleList.Where(a => a.Id.ToString() == input.UserRole).ToList())
            {
                var role = await _roleManager.GetRoleByIdAsync(roleName.Id);
                user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, role.Id));
            }
            user.ShouldChangePasswordOnNextLogin = true;
            CheckErrors(await UserManager.CreateAsync(user));
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new user's Id.


            //User Company

            //if (input.Facility != null)
            //{
            //    var employeeUser = await UserManager.FindByEmailAsync(user.EmailAddress);
            //    if (employeeUser != null)
            //    {
            //        var userCompanyId = 0;
            //        var company = _userCompanyAppService.GetCompanyByFacility(int.Parse(input.Facility));
            //        if (input.Facility != null && int.Parse(input.Facility) > 0 && company != null)
            //        {
            //            userCompanyId = _userCompanyAppService.InsertUserCompany(new UserCompany
            //            {
            //                UserId = employeeUser.Id,
            //                CompanyId = company.Id,
            //                FacilityId = int.Parse(input.Facility)
            //            });
            //        }
            //        if (userCompanyId == 0)
            //            _userCompanyAppService.InsertUserCompany(AbpSession.UserId.Value, employeeUser.Id);
            //    }
            //}

            //Logged in user company
            var loggedInUserCompany = _userCompanyAppService.GetUserCompany(AbpSession.UserId);
            if ((input.FacilityIds == null || input.FacilityIds.Length == 0) && !string.IsNullOrEmpty(input.Facility))
            {

                var company = new Company
                {
                    CompanyName = input.Facility,
                    CompanyStatusId = 1,
                    BillTo = (int)BillToEnum.Patient,
                    DeliveryTypeId = (int)DeliveryTypeEnum.DeliveryToPatientHome
                };

                var companyId = await _companyAppService.Insert(company);

                var facility = new Facility
                {
                    FacilityName = input.Facility,
                    FacilityStatusId = 1,
                    Company = company,
                };

                var facilityId = await _facilityAppService.Insert(facility);


                var userCompanyId = _userCompanyAppService.InsertUserCompany(new UserCompany
                {
                    UserId = user.Id,
                    CompanyId = companyId,
                    FacilityId = facilityId,
                    IsDefaultFacility = true

                });
                if (userCompanyId == 0)
                    _userCompanyAppService.InsertUserCompany(AbpSession.UserId.Value, user.Id);

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
                _userCompanyAppService.InsertUserCompany(AbpSession.UserId.Value, user.Id);

            if (true)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(
                    user,
                    AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId),
                    randomPassword
                );
            }
        }
        [AbpAuthorize(AppPermissions.Pages_Employee_Delete)]
        public async Task DeleteUser(EntityDto<long> input)
        {
            if (input.Id == AbpSession.GetUserId())
            {
                throw new UserFriendlyException(L("YouCanNotDeleteOwnAccount"));
            }

            var user = await UserManager.GetUserByIdAsync(input.Id);
            CheckErrors(await UserManager.DeleteAsync(user));
        }
    }
}

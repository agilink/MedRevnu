using ATI;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.UI;
using ATI.Admin.Domain.Entities;
using ATI.Authorization.Roles;
using ATI.Authorization.Users;
using ATI.Revenue.Application.Physicians.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ATI.Revenue.Application.Physicians
{
    /// <summary>
    /// Manages the physician roster.
    /// </summary>
    /// <remarks>
    /// Physicians are ADM.Personnel rows; the revenue transaction form selects from them
    /// and takes each transaction's hospital from the physician's facility. Until now
    /// there was no UI for them anywhere in the solution - the Admin module has no web
    /// project - so the roster could only be populated by running a seed script. That
    /// made it impossible to onboard a new surgeon without a developer.
    ///
    /// The service lives in the Revenue module because that is where the working web
    /// layer is and where physicians are actually used.
    /// </remarks>
    public class PhysiciansAppService : ApplicationService, IPhysiciansAppService
    {
        private const string DefaultPassword = "Welcome01";
        private const string FallbackEmailDomain = "@medrevenue.com";
        private const string PhysicianRoleName = "Physician";

        private readonly IRepository<Personnel, int> _personnelRepository;
        private readonly IRepository<Facility, int> _facilityRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;

        public PhysiciansAppService(
            IRepository<Personnel, int> personnelRepository,
            IRepository<Facility, int> facilityRepository,
            IRepository<User, long> userRepository,
            UserManager userManager,
            RoleManager roleManager)
        {
            // ABP's L() throws unless the source is named, so every localised message in
            // this service would have been an exception instead of a message.
            LocalizationSourceName = ATIConsts.LocalizationSourceName;

            _personnelRepository = personnelRepository;
            _facilityRepository = facilityRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<PagedResultDto<PhysicianDto>> GetAll(GetAllPhysiciansInput input)
        {
            var baseQuery = CreateFilteredQuery(input);

            var totalCount = await baseQuery.CountAsync();

            var sortedQuery = string.IsNullOrWhiteSpace(input.Sorting)
                ? baseQuery.OrderBy(p => p.LAST_NAME).ThenBy(p => p.FIRST_NAME)
                : baseQuery.OrderBy(input.Sorting);

            var dtos = await Project(sortedQuery.PageBy(input)).ToListAsync();

            return new PagedResultDto<PhysicianDto>(totalCount, dtos);
        }

        public async Task<GetPhysicianForViewDto> GetPhysicianForView(int id)
        {
            var dto = await Project(_personnelRepository.GetAll().Where(p => p.Id == id)).FirstOrDefaultAsync();

            if (dto == null)
                throw new UserFriendlyException("Physician not found");

            return new GetPhysicianForViewDto { Physician = dto };
        }

        public async Task<GetPhysicianForEditOutput> GetPhysicianForEdit(EntityDto<int> input)
        {
            var entity = await _personnelRepository.GetAll().FirstOrDefaultAsync(p => p.Id == input.Id);

            if (entity == null)
                throw new UserFriendlyException("Physician not found");

            return new GetPhysicianForEditOutput
            {
                Physician = new CreateOrEditPhysicianDto
                {
                    Id = entity.Id,
                    FirstName = entity.FIRST_NAME,
                    MiddleName = entity.MIDDLE_NAME,
                    LastName = entity.LAST_NAME,
                    HospitalId = entity.FacilityId,
                    EmployeeId = entity.EMPLOYEE_ID,
                    EmailWork = entity.EMAIL_WORK,
                    MobileNumber = entity.NUMBER_MOBILE,
                    PersonnelType = entity.PersonnelTypeID,
                    EmployeeStatus = entity.EmployeeStatusID,
                    Notes = entity.NOTES
                }
            };
        }

        public async Task<PhysicianDto> CreateOrEdit(CreateOrEditPhysicianDto input)
        {
            await EnsureHospitalExists(input.HospitalId);

            var entity = input.Id == 0
                ? new Personnel()
                : await _personnelRepository.GetAsync(input.Id);

            entity.FIRST_NAME = input.FirstName?.Trim();
            entity.MIDDLE_NAME = input.MiddleName?.Trim();
            entity.LAST_NAME = input.LastName?.Trim();
            entity.FacilityId = input.HospitalId;
            entity.EMPLOYEE_ID = input.EmployeeId?.Trim();
            entity.EMAIL_WORK = input.EmailWork?.Trim();
            entity.NUMBER_MOBILE = input.MobileNumber?.Trim();
            entity.PersonnelTypeID = input.PersonnelType;
            entity.EmployeeStatusID = input.EmployeeStatus;
            entity.NOTES = input.Notes;

            // FIRST_NAME_PROPER is a display copy of the first name elsewhere in the
            // schema; kept in step so it does not drift from what was entered here.
            entity.FIRST_NAME_PROPER = entity.FIRST_NAME;

            int id;
            if (input.Id == 0)
            {
                id = await _personnelRepository.InsertAndGetIdAsync(entity);
            }
            else
            {
                await _personnelRepository.UpdateAsync(entity);
                id = entity.Id;
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return await Project(_personnelRepository.GetAll().Where(p => p.Id == id)).FirstOrDefaultAsync();
        }

        public async Task Delete(EntityDto<int> input)
        {
            // Personnel is soft-deleted, so transactions already recorded against this
            // physician keep their reference and historical reports stay intact.
            await _personnelRepository.DeleteAsync(input.Id);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private async Task EnsureHospitalExists(int? hospitalId)
        {
            if (!hospitalId.HasValue)
            {
                return;
            }

            if (!await _facilityRepository.GetAll().AnyAsync(f => f.Id == hospitalId.Value))
            {
                throw new UserFriendlyException("The selected hospital no longer exists.");
            }
        }

        /// <summary>
        /// Gives a physician a login.
        /// </summary>
        /// <remarks>
        /// The username is the first letter of the first name followed by the surname with
        /// nothing between them - Eric Thomassee becomes "ethomassee". Where that is
        /// already taken a number is appended, because two physicians can easily reduce to
        /// the same username and a duplicate would otherwise fail at the database.
        ///
        /// Most physicians on the roster have no email address, and a user cannot be
        /// created without one, so a missing address falls back to
        /// {username}@medrevenue.com. That address cannot receive mail, so password reset
        /// will not work for it until a real one is set on the physician.
        ///
        /// The account is created with the agreed default password and is not asked to
        /// change it on first login.
        /// </remarks>
        public async Task<CreatePhysicianUserOutput> CreateUserForPhysician(EntityDto<int> input)
        {
            var physician = await _personnelRepository.FirstOrDefaultAsync(p => p.Id == input.Id);

            if (physician == null)
            {
                throw new UserFriendlyException(L("PhysicianNotFound"));
            }

            if (physician.UserId.HasValue)
            {
                // The grid hides the action once a login exists; this catches a second
                // click that got through before the row refreshed.
                throw new UserFriendlyException(L("PhysicianAlreadyHasUser"));
            }

            if (string.IsNullOrWhiteSpace(physician.LAST_NAME))
            {
                throw new UserFriendlyException(L("PhysicianNeedsNameForUser"));
            }

            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name == PhysicianRoleName && r.TenantId == AbpSession.TenantId);

            if (role == null)
            {
                throw new UserFriendlyException(L("PhysicianRoleMissing"));
            }

            var userName = await BuildAvailableUserName(physician.FIRST_NAME, physician.LAST_NAME);

            var emailAddress = string.IsNullOrWhiteSpace(physician.EMAIL_WORK)
                ? userName + FallbackEmailDomain
                : physician.EMAIL_WORK.Trim();

            var user = new User
            {
                TenantId = AbpSession.TenantId,
                UserName = userName,
                Name = string.IsNullOrWhiteSpace(physician.FIRST_NAME) ? userName : physician.FIRST_NAME.Trim(),
                Surname = physician.LAST_NAME.Trim(),
                EmailAddress = emailAddress,
                IsActive = true,

                // No activation email is sent - the address is often a placeholder - so
                // the account has to start out confirmed or nobody could sign in.
                IsEmailConfirmed = true,
                ShouldChangePasswordOnNextLogin = false
            };

            // Applies the tenant's own password rules before the password is accepted.
            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

            CheckIdentity(await _userManager.CreateAsync(user, DefaultPassword));
            await CurrentUnitOfWork.SaveChangesAsync(); // so the new user has an Id

            CheckIdentity(await _userManager.AddToRoleAsync(user, role.Name));

            physician.UserId = user.Id;
            await _personnelRepository.UpdateAsync(physician);

            return new CreatePhysicianUserOutput
            {
                UserId = user.Id,
                UserName = user.UserName,
                EmailAddress = user.EmailAddress,
                Password = DefaultPassword,
                UsedFallbackEmail = string.IsNullOrWhiteSpace(physician.EMAIL_WORK)
            };
        }

        /// <summary>First letter of the first name plus the surname, no separator.</summary>
        private static string BuildUserName(string firstName, string lastName)
        {
            var initial = new string((firstName ?? "").Where(char.IsLetterOrDigit).Take(1).ToArray());
            var surname = new string((lastName ?? "").Where(char.IsLetterOrDigit).ToArray());

            return (initial + surname).ToLowerInvariant();
        }

        private async Task<string> BuildAvailableUserName(string firstName, string lastName)
        {
            var baseName = BuildUserName(firstName, lastName);

            if (string.IsNullOrEmpty(baseName))
            {
                throw new UserFriendlyException(L("PhysicianNeedsNameForUser"));
            }

            var candidate = baseName;

            // J. Smith and Jane Smith both reduce to "jsmith", so suffix until free.
            for (var suffix = 2; await UserNameExists(candidate); suffix++)
            {
                candidate = baseName + suffix;
            }

            return candidate;
        }

        private Task<bool> UserNameExists(string userName)
        {
            return _userRepository.GetAll().AnyAsync(u => u.UserName == userName);
        }

        private void CheckIdentity(IdentityResult result)
        {
            result.CheckErrors(LocalizationManager);
        }

        private IQueryable<PhysicianDto> Project(IQueryable<Personnel> query)
        {
            // Correlated so the grid can show the username in one round trip rather than
            // a second lookup per row.
            var users = _userRepository.GetAll();

            return query.Select(p => new PhysicianDto
            {
                Id = p.Id,
                FirstName = p.FIRST_NAME ?? "",
                MiddleName = p.MIDDLE_NAME ?? "",
                LastName = p.LAST_NAME ?? "",
                FullName = ((p.FIRST_NAME ?? "") + " " + (p.LAST_NAME ?? "")).Trim(),
                HospitalId = p.FacilityId,
                HospitalName = p.Facility != null ? (p.Facility.FacilityName ?? "") : "",
                EmployeeId = p.EMPLOYEE_ID ?? "",
                EmailWork = p.EMAIL_WORK ?? "",
                MobileNumber = p.NUMBER_MOBILE ?? "",
                PersonnelType = p.PersonnelTypeID,
                EmployeeStatus = p.EmployeeStatusID,
                Notes = p.NOTES ?? "",
                UserId = p.UserId,
                UserName = p.UserId.HasValue
                    ? users.Where(u => u.Id == p.UserId.Value).Select(u => u.UserName).FirstOrDefault()
                    : null
            });
        }

        private IQueryable<Personnel> CreateFilteredQuery(GetAllPhysiciansInput input)
        {
            var query = _personnelRepository.GetAll();

            if (input.UnassignedHospitalOnly)
            {
                query = query.Where(p => p.FacilityId == null);
            }
            else if (input.HospitalIdFilter.HasValue)
            {
                query = query.Where(p => p.FacilityId == input.HospitalIdFilter.Value);
            }

            if (input.EmployeeStatusFilter.HasValue)
            {
                query = query.Where(p => p.EmployeeStatusID == input.EmployeeStatusFilter.Value);
            }

            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                var filter = input.Filter.Trim();
                query = query.Where(p =>
                    (p.FIRST_NAME != null && p.FIRST_NAME.Contains(filter)) ||
                    (p.LAST_NAME != null && p.LAST_NAME.Contains(filter)) ||
                    (p.EMPLOYEE_ID != null && p.EMPLOYEE_ID.Contains(filter)) ||
                    (p.EMAIL_WORK != null && p.EMAIL_WORK.Contains(filter)));
            }

            return query;
        }
    }
}

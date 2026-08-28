using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using ATI.Admin.Domain.Entities;
using ATI.Authorization.Roles;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Abp.Authorization.Users;

namespace ATI.Revenue.Application.Authorization
{
    /// <summary>
    /// How much revenue data the signed-in user is allowed to see.
    /// </summary>
    public class PhysicianDataScope
    {
        /// <summary>
        /// True when the user is a physician and must only see one hospital.
        /// </summary>
        public bool IsRestricted { get; set; }

        /// <summary>
        /// The only hospital they may see. Null while <see cref="IsRestricted"/> is true
        /// means they may see nothing at all, which is not the same as no filter.
        /// </summary>
        public int? HospitalId { get; set; }

        /// <summary>
        /// The personnel record behind this login, when there is one. Lets a screen
        /// preselect the signed-in physician instead of offering the whole roster.
        /// Null for a user holding the Physician role with no personnel record.
        /// </summary>
        public int? PhysicianId { get; set; }

        /// <summary>
        /// Restricted, but with no hospital on their physician record - so there is
        /// nothing they are allowed to see until someone assigns one.
        /// </summary>
        public bool SeesNothing => IsRestricted && !HospitalId.HasValue;

        public static PhysicianDataScope Unrestricted()
        {
            return new PhysicianDataScope { IsRestricted = false };
        }

        /// <summary>
        /// Narrows a requested hospital filter to what the user is allowed to ask for.
        /// A restricted user's own hospital always wins over whatever the page sent.
        /// </summary>
        public int? ApplyTo(int? requestedHospitalId)
        {
            return IsRestricted ? HospitalId : requestedHospitalId;
        }
    }

    public interface IPhysicianDataScopeProvider
    {
        Task<PhysicianDataScope> GetAsync();
    }

    /// <summary>
    /// Works out whether the signed-in user is a physician, and if so which hospital.
    /// </summary>
    /// <remarks>
    /// Restricted on either of two signals, and the union is deliberate:
    ///
    ///  - the user is linked to a personnel record (ADM.Personnel.UserId), which is how
    ///    logins created from the physician grid are recorded;
    ///  - the user is in the Physician role.
    ///
    /// Either alone would leave a gap. Someone given the Physician role by hand without a
    /// personnel record would otherwise be unrestricted and see every hospital, which is
    /// the failure that actually matters; and a physician login later given a second role
    /// would otherwise shed its restriction. Where the two disagree the answer is always
    /// the narrower one, so a mismatch costs someone access rather than leaking data.
    /// </remarks>
    public class PhysicianDataScopeProvider : IPhysicianDataScopeProvider, ITransientDependency
    {
        private const string PhysicianRoleName = "Physician";

        private readonly IAbpSession _abpSession;
        private readonly IRepository<Personnel, int> _personnelRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;

        public PhysicianDataScopeProvider(
            IAbpSession abpSession,
            IRepository<Personnel, int> personnelRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository)
        {
            _abpSession = abpSession;
            _personnelRepository = personnelRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
        }

        public async Task<PhysicianDataScope> GetAsync()
        {
            var userId = _abpSession.UserId;

            if (!userId.HasValue)
            {
                return PhysicianDataScope.Unrestricted();
            }

            var physician = await _personnelRepository.GetAll()
                .Where(p => p.UserId == userId.Value)
                .Select(p => new { p.Id, p.FacilityId })
                .FirstOrDefaultAsync();

            if (physician != null)
            {
                return new PhysicianDataScope
                {
                    IsRestricted = true,
                    HospitalId = physician.FacilityId,
                    PhysicianId = physician.Id
                };
            }

            // No personnel record. Still restricted if they hold the Physician role - and
            // with no hospital to go on, that means they see nothing.
            var inPhysicianRole = await _userRoleRepository.GetAll()
                .Where(ur => ur.UserId == userId.Value)
                .AnyAsync(ur => _roleRepository.GetAll()
                    .Any(r => r.Id == ur.RoleId && r.Name == PhysicianRoleName));

            return inPhysicianRole
                ? new PhysicianDataScope { IsRestricted = true, HospitalId = null }
                : PhysicianDataScope.Unrestricted();
        }
    }
}

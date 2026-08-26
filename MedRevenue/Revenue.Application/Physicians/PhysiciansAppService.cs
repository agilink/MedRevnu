using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using ATI.Admin.Domain.Entities;
using ATI.Revenue.Application.Physicians.Dtos;
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
        private readonly IRepository<Personnel, int> _personnelRepository;
        private readonly IRepository<Facility, int> _facilityRepository;

        public PhysiciansAppService(
            IRepository<Personnel, int> personnelRepository,
            IRepository<Facility, int> facilityRepository)
        {
            _personnelRepository = personnelRepository;
            _facilityRepository = facilityRepository;
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

        private static IQueryable<PhysicianDto> Project(IQueryable<Personnel> query)
        {
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
                Notes = p.NOTES ?? ""
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

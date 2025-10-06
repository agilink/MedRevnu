using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization.Users;
using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MimeTypes;
using ATI.Admin.Application;
using ATI.Authorization;
using ATI.Authorization.Roles;
using ATI.Authorization.Users;
using ATI.Pharmacy.Application;
using ATI.Pharmacy.Dtos;
using ATI.Pharmacy.Web.PageModel.Patients;
using ATI.Pharmacy.Web.PageModel.Prescriptions;
using ATI.Storage;
using ATI.Web.Controllers;
using k8s.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ATI.Pharmacy.Web.Controllers
{
    [Area("Pharmacy")]
    public class EmployeesController : ATIControllerBase
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IUserCompanyAppService _userCompanyAppService;
        private readonly IUserExtendedAppService _userExtendedAppService;
        private readonly IUnitOfWorkManager _unitOfWork;
        public EmployeesController(UserManager userManager, RoleManager roleManager, IRepository<Role> roleRepo, IUserCompanyAppService userCompanyAppService, IUserExtendedAppService userExtendedAppService
            , IUnitOfWorkManager unitOfWork)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userCompanyAppService = userCompanyAppService;
            _userExtendedAppService = userExtendedAppService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var excludedIds = new int[] { 1, 2, 4 };
            var viewModel = new PharmacyUserViewModel();
            UnitOfWorkOptions option = new UnitOfWorkOptions();
            option.Scope = System.Transactions.TransactionScopeOption.RequiresNew;
            option.IsTransactional = true;
            using (var uom = _unitOfWork.Begin(option))
            {
                viewModel.Role = _roleManager.Roles.Where(a => !excludedIds.Any(b => b == a.Id))
                    .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.DisplayName }).ToList();
                uom.Complete();
            }
            using (var uom = _unitOfWork.Begin(option))
            {
                viewModel.Facility = await _userCompanyAppService.FacilitySelectList((int)AbpSession.UserId.Value, false);
                uom.Complete();
            }
            return View(viewModel);
        }
        public async Task<PartialViewResult> CreateOrEdit(int? id)
        {
            // Fetch roles asynchronously with only required fields

            var userId = (int)AbpSession.UserId;

            CreateOrEditUserInputDto viewModel;

            var excludedIds = new int[] { 1, 2, 4 };
            var roleList = _roleManager.Roles.Where(a => !excludedIds.Any(b => b == a.Id)).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.DisplayName.ToString() }).ToList();

            //var medicines = await _madicationsAppService.GetMedications(6, 1);
            if (id.HasValue)
            {
                var userCompanies = _userCompanyAppService.GetUserCompanies(id);
                viewModel = await _userExtendedAppService.GetEmployeeForEdit(new EntityDto { Id = (int)id });
                viewModel.IsEditMode = id.HasValue;
                viewModel.RoleList = roleList;
                viewModel.FacilityList = await _userCompanyAppService.FacilitySelectList((int)id, false);
                viewModel.UserId = (int)id.Value;
                viewModel.FacilityIds = userCompanies?.Select(a => a.FacilityId)?.ToArray() ?? new int[0];
            }
            else
            {
                viewModel = new CreateOrEditUserInputDto()
                {
                    IsEditMode = id.HasValue, // Set IsEditMode based on the presence of id
                    RoleList = roleList,
                    FacilityList = await _userCompanyAppService.FacilitySelectList(userId, false),
                    UserId = 0
                };
            }

            var roles = await _userManager.GetRolesAsync(_userManager.GetUserById(AbpSession.UserId ?? 0));
            viewModel.IsPharmacyLogin = roles.Contains("3940adad1759401aab8d8a4b37daec8c") || roles.Contains("292b594325de432ba087f999fb429e36");//Pharmacy

            // Return the partial view with the model
            return PartialView("_PharmacyUserDetails", viewModel);
        }
    }
}

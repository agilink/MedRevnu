using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Domain.Repositories;
using ATI.Admin.Domain.Entities;
using ATI.Authorization;
using ATI.Revenue.Application.Physicians;
using ATI.Revenue.Application.Physicians.Dtos;
using ATI.Revenue.Web.PageModel.Physicians;
using ATI.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    [AbpMvcAuthorize(AppPermissions.Pages_Revenue_Physicians)]
    public class PhysiciansController : ATIControllerBase
    {
        private readonly IPhysiciansAppService _physiciansAppService;
        private readonly IRepository<Facility, int> _facilityRepository;

        public PhysiciansController(
            IPhysiciansAppService physiciansAppService,
            IRepository<Facility, int> facilityRepository)
        {
            _physiciansAppService = physiciansAppService;
            _facilityRepository = facilityRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Hospitals = await GetHospitalSelectList();
            return View();
        }

        public async Task<IActionResult> CreateOrEditModal(int? id)
        {
            CreateOrEditPhysicianModalViewModel viewModel;

            if (id.HasValue)
            {
                var output = await _physiciansAppService.GetPhysicianForEdit(new EntityDto<int> { Id = id.Value });
                viewModel = new CreateOrEditPhysicianModalViewModel
                {
                    Physician = output.Physician,
                    IsEditMode = true
                };
            }
            else
            {
                viewModel = new CreateOrEditPhysicianModalViewModel
                {
                    Physician = new CreateOrEditPhysicianDto(),
                    IsEditMode = false
                };
            }

            ViewBag.Hospitals = await GetHospitalSelectList();

            return PartialView("_CreateOrEditModal", viewModel);
        }

        private async Task<SelectList> GetHospitalSelectList()
        {
            var hospitals = await _facilityRepository.GetAll()
                .OrderBy(f => f.FacilityName)
                .Select(f => new { f.Id, Name = f.FacilityName ?? "" })
                .ToListAsync();

            return new SelectList(hospitals, "Id", "Name");
        }
    }
}

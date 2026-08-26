using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using ATI.Authorization;
using ATI.Revenue.Application.Hospitals;
using ATI.Revenue.Application.Hospitals.Dtos;
using ATI.Revenue.Web.PageModel.Hospitals;
using ATI.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Abp.Domain.Repositories;
using ATI.Admin.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    [AbpMvcAuthorize(AppPermissions.Pages_Revenue_Hospitals)]
    public class HospitalsController : ATIControllerBase
    {
        private readonly IHospitalsAppService _hospitalsAppService;
        private readonly IRepository<Company, int> _companyRepository;

        public HospitalsController(
            IHospitalsAppService hospitalsAppService,
            IRepository<Company, int> companyRepository)
        {
            _hospitalsAppService = hospitalsAppService;
            _companyRepository = companyRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        private async Task<SelectList> GetCompanySelectList()
        {
            var companies = await _companyRepository.GetAll()
                .OrderBy(c => c.CompanyName)
                .Select(c => new { c.Id, Name = c.CompanyName ?? "" })
                .ToListAsync();

            return new SelectList(companies, "Id", "Name");
        }

        public async Task<IActionResult> CreateOrEditModal(int? id)
        {
            CreateOrEditHospitalModalViewModel viewModel;

            if (id.HasValue)
            {
                var output = await _hospitalsAppService.GetHospitalForEdit(new EntityDto<int> { Id = id.Value });
                viewModel = new CreateOrEditHospitalModalViewModel
                {
                    Hospital = output.Hospital,
                    IsEditMode = true
                };
            }
            else
            {
                viewModel = new CreateOrEditHospitalModalViewModel
                {
                    Hospital = new CreateOrEditHospitalDto(),
                    IsEditMode = false
                };
            }

            ViewBag.Companies = await GetCompanySelectList();

            return PartialView("_CreateOrEditModal", viewModel);
        }
    }
}

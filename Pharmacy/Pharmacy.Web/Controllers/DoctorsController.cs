namespace ATI.Pharmacy.Web.PageModel.Doctors;
using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.BackgroundJobs;
using Abp.MimeTypes;
using ATI.Admin.Application;
using ATI.Admin.Application.Addresses.Dtos;
using ATI.Admin.Application.Companies.Dtos;
using ATI.Admin.Application.Facilities.Dtos;
using ATI.Admin.Domain.Entities;
using ATI.Authorization;
using ATI.Authorization.Users;
using ATI.DataImporting.Excel;
using ATI.Pharmacy.Dtos;
using ATI.Storage;
using ATI.Web.Controllers;
using Microsoft.AspNetCore.Mvc;


[Area("Pharmacy")]
[AbpMvcAuthorize(AppPermissions.Pages_Doctors)]
public class DoctorsController : ExcelImportControllerBase
{
    private readonly IDoctorsAppService _doctorsAppService;

    protected readonly IBinaryObjectManager _binaryObjectManager;
    protected readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IAddressAppService _addressAppService;
    private readonly IMimeTypeMap _mimeTypeMap;
    private readonly UserManager _userManager;
    private readonly IUserCompanyAppService _userCompanyAppService;
    public override string ImportExcelPermission => AppPermissions.Pages_Doctors_Create;

    public DoctorsController(IDoctorsAppService doctorsAppService, IBinaryObjectManager binaryObjectManager, IMimeTypeMap mimeTypeMap,
        IBackgroundJobManager backgroundJobManager, IAddressAppService addressAppService, UserManager userManager,
        IUserCompanyAppService userCompanyAppService) : base(binaryObjectManager, backgroundJobManager)
    {
        _doctorsAppService = doctorsAppService;
        _mimeTypeMap = mimeTypeMap;
        _binaryObjectManager = binaryObjectManager;
        _backgroundJobManager = backgroundJobManager;
        _addressAppService = addressAppService;
        _userManager = userManager;
        _userCompanyAppService = userCompanyAppService;
    }

    public async Task<ActionResult> Index()
    {
        var model = new DoctorsViewModel
        {
            FilterText = ""
        };
        var doctor = await _doctorsAppService.GetDoctorByUserId((int)AbpSession.UserId);
        var userCompanies = _userCompanyAppService.GetUserCompanies((int)AbpSession.UserId);
        if (userCompanies.Count() > 1)
        {
            model.FacilityList = await _userCompanyAppService.UserFacilitySelectList((int)AbpSession.UserId, true);
        }
        if (userCompanies.Count() > 0)
        {
            model.CompanyList = await _userCompanyAppService.CompanySelectList((int)AbpSession.UserId, false);
            model.CompanyId = 0; //userCompanies.FirstOrDefault(a => a.IsDefaultFacility)?.CompanyId ?? userCompanies.FirstOrDefault()?.CompanyId;
        }
        return View(model);
    }

    [AbpMvcAuthorize(AppPermissions.Pages_Doctors_Create, AppPermissions.Pages_Doctors_Edit)]
    public async Task<PartialViewResult> CreateOrEditModal(int? id)
    {
        GetDoctorForEditOutput getDoctorForEditOutput;

        if (id.HasValue)
        {
            getDoctorForEditOutput = await _doctorsAppService.GetDoctorForEdit(new EntityDto { Id = (int)id });
        }
        else
        {
            getDoctorForEditOutput = new GetDoctorForEditOutput
            {
                Doctor = new CreateOrEditDoctorDto(),
                User = new CreateOrEditUserDto(),
                Address = new AddressDto(),
                Facilities = new List<FacilityDto>()
            };
        }
        var currentUserId = AbpSession.UserId;
        //  var roles = await _userManager.GetRolesAsync(_userManager.GetUserById(currentUserId ?? 0));

        //if (getDoctorForEditOutput.Address == null)
        //    getDoctorForEditOutput.Address = new AddressDto();
        //getDoctorForEditOutput.Address.States = await _addressAppService.SelectAllStates();
        //getDoctorForEditOutput.Address.IsFaxRequired = true;
        var viewModel = new CreateOrEditDoctorModalViewModel()
        {
            Doctor = getDoctorForEditOutput.Doctor,
            User = getDoctorForEditOutput.User,
            //Address = getDoctorForEditOutput.Address,
            FacilityIds = getDoctorForEditOutput.Facilities.Select(a => a.Id)?.ToArray() ?? new int[0],
            Facilities = await _userCompanyAppService.FacilitySelectList(getDoctorForEditOutput.User.Id ?? 0, true),
            SignaturePermission = getDoctorForEditOutput.User.Id == currentUserId
            // IsPharmacyLogin = roles.Contains("3940adad1759401aab8d8a4b37daec8c") || roles.Contains("292b594325de432ba087f999fb429e36")//Pharmacy
        };

        return PartialView("_CreateOrEditModal", viewModel);
    }
    [HttpPost]
    public async Task<IActionResult> UploadSignature(IFormFile file)
    {
        var binaryObject = new BinaryObject();
        using (var memoryStream = new MemoryStream())
        {
            // Copy file content to memory stream
            await file.CopyToAsync(memoryStream);

            var bytes = memoryStream.ToArray();
            binaryObject = new BinaryObject()
            {
                TenantId = AbpSession.TenantId,
                Bytes = bytes,
                Description = file.Name
            };
            await _binaryObjectManager.SaveAsync(binaryObject);
        }

        return Ok(new
        {
            binaryObjectId = binaryObject.Id
        });
    }

    public async Task<PartialViewResult> ViewDoctorModal(int id)
    {
        var getDoctorForViewDto = await _doctorsAppService.GetDoctorForView(id);

        var model = new DoctorViewModel()
        {
            Doctor = getDoctorForViewDto.Doctor
        };

        return PartialView("_ViewDoctorModal", model);
    }

    public override async Task EnqueueExcelImportJobAsync(ImportFromExcelJobArgs args)
    {
        await BackgroundJobManager.EnqueueAsync<ImportDoctorsToExcelJob, ImportFromExcelJobArgs>(args);
    }

    public async Task<PartialViewResult> ExcelColumnSelectionModal(long? id)
    {
        var output = await _doctorsAppService.GetDoctorExcelColumnsToExcel();
        var viewModel = new DoctorExcelColumnSelectionViewModel
        {
            DoctorExcelColumns = output
        };

        return PartialView("_ExcelColumnSelectionModal", viewModel);
    }


    public LocalRedirectResult ChangeFacility(int id, string returnurl)
    {
        _doctorsAppService.SetDefaultFacility(id);
        return LocalRedirect(returnurl);
    }
}
using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.BackgroundJobs;
using Abp.Domain.Uow;
using ATI.Admin.Application;
using ATI.Admin.Application.Addresses.Dtos;
using ATI.Authorization;
using ATI.DataImporting.Excel;
using ATI.Pharmacy.Application.Alergy;
using ATI.Pharmacy.Dtos;
using ATI.Pharmacy.Web.PageModel.Patients;
using ATI.Storage;
using ATI.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ATI.Pharmacy.Web.Controllers;

[Area("Pharmacy")]
[AbpMvcAuthorize(AppPermissions.Pages_Patients)]
public class PatientsController : ExcelImportControllerBase
{
    private readonly IPatientsAppService _patientsAppService;
    private readonly IAddressAppService _addressAppService;

    protected readonly IBinaryObjectManager _binaryObjectManager;
    protected readonly IBackgroundJobManager _backgroundJobManager;
    protected readonly IAllergyAppService _allergyAppService;
    protected readonly IUnitOfWork _unitOfWork;
    
    protected readonly IDoctorsAppService _doctorsAppService;
    protected readonly IUserCompanyAppService _userCompanyAppService;


    public override string ImportExcelPermission => AppPermissions.Pages_Patients_Create;

    public PatientsController(IPatientsAppService patientsAppService, IAddressAppService addressAppService, IBinaryObjectManager binaryObjectManager,
        IBackgroundJobManager backgroundJobManager, IAllergyAppService allergyAppService, IDoctorsAppService doctorsAppService, IUserCompanyAppService userCompanyAppService) : base(binaryObjectManager, backgroundJobManager)
    {
        _patientsAppService = patientsAppService;
        _addressAppService = addressAppService;
        _binaryObjectManager = binaryObjectManager;
        _backgroundJobManager = backgroundJobManager;
        _allergyAppService = allergyAppService;
        _doctorsAppService = doctorsAppService;
        _userCompanyAppService = userCompanyAppService;
    }

    public async Task<ActionResult> Index()
    {
        var model = new PatientsViewModel
        {
            FilterText = ""
        };
        //var doctor = await _doctorsAppService.GetDoctorByUserId((int)AbpSession.UserId);
        //var userCompanies = _userCompanyAppService.GetUserCompanies((int)AbpSession.UserId);
        //if (userCompanies.Count() > 1)
        //{
        //    model.FacilityList = await _userCompanyAppService.UserFacilitySelectList((int)AbpSession.UserId, true);
        //}
        return View(model);
    }

    [AbpMvcAuthorize(AppPermissions.Pages_Patients_Create, AppPermissions.Pages_Patients_Edit)]
    public async Task<PartialViewResult> CreateOrEditModal(int? id)
    {
        GetPatientForEditOutput getPatientForEditOutput;

        if (id.HasValue)
        {
            getPatientForEditOutput = await _patientsAppService.GetPatientForEdit(new EntityDto { Id = (int)id });
        }
        else
        {
            getPatientForEditOutput = new GetPatientForEditOutput
            {
                Patient = new CreateOrEditPatientDto(),
                Address = new AddressDto()
            };
            getPatientForEditOutput.Patient.DateOfBirth = null;

        }
        getPatientForEditOutput.Address.States = await _addressAppService.SelectAllStates();
        getPatientForEditOutput.Patient.Allergies = await _allergyAppService.GetSelectListItem();

        var usersOfSameFacility = _userCompanyAppService.GetOtherUsersOfCompany(AbpSession.UserId.Value);


        var doctors = await _doctorsAppService.GetAllDoctor(usersOfSameFacility);
        var viewModel = new CreateOrEditPatientModalViewModel()
        {
            Patient = getPatientForEditOutput.Patient,
            Address = getPatientForEditOutput.Address,
            Doctors = doctors,
            DoctorId = getPatientForEditOutput.Patient.DoctorId
        };

        if (doctors.Count > 2)
            viewModel.MultipleDoctorAvailable = true;

        return PartialView("_CreateOrEditModal", viewModel);
    }

    public async Task<PartialViewResult> CreateOrEditPatient(int? id)
    {
        GetPatientForEditOutput getPatientForEditOutput;

        if (id.HasValue && id > 0)
        {
            getPatientForEditOutput = await _patientsAppService.GetPatientForEdit(new EntityDto { Id = (int)id });
        }
        else
        {
            getPatientForEditOutput = new GetPatientForEditOutput
            {
                Patient = new CreateOrEditPatientDto(),
                Address = new AddressDto()
            };
            getPatientForEditOutput.Patient.DateOfBirth = null;

        }
        getPatientForEditOutput.Address.States = await _addressAppService.SelectAllStates();
        getPatientForEditOutput.Patient.Allergies = await _allergyAppService.GetSelectListItem();
        var viewModel = new CreateOrEditPatientModalViewModel()
        {
            Patient = getPatientForEditOutput.Patient,
            Address = getPatientForEditOutput.Address,
        };

        return PartialView("_CreateOrEditPatient", viewModel);
    }

    public async Task<PartialViewResult> ViewPatientModal(int id)
    {
        var getPatientForViewDto = await _patientsAppService.GetPatientForView(id);

        var model = new PatientViewModel()
        {
            Patient = getPatientForViewDto.Patient
        };

        return PartialView("_ViewPatientModal", model);
    }

    public override async Task EnqueueExcelImportJobAsync(ImportFromExcelJobArgs args)
    {

        //await BackgroundJobManager.EnqueueAsync<ImportPatientsToExcelJob, ImportFromExcelJobArgs>(args);
        await _patientsAppService.GetPatientExcelColumnsToExcel();
    }

    public async Task<PartialViewResult> ExcelColumnSelectionModal(long? id)
    {
        var output = await _patientsAppService.GetPatientExcelColumnsToExcel();
        var viewModel = new PatientExcelColumnSelectionViewModel
        {
            PatientExcelColumns = output
        };

        return PartialView("_ExcelColumnSelectionModal", viewModel);
    }
}
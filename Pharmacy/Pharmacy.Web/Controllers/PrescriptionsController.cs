using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using AngleSharp.Text;
using ATI.Admin.Application;
using ATI.Admin.Application.Addresses.Dtos;
using ATI.Authorization;
using ATI.Authorization.Users;
using ATI.Pharmacy.Application.Alergy;
using ATI.Pharmacy.Domain.Entities;
using ATI.Pharmacy.Dtos;

using ATI.Pharmacy.Web.PageModel.Patients;
using ATI.Pharmacy.Web.PageModel.Prescriptions;
using ATI.Storage;
using ATI.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Tweetinvi.Core.Events;

namespace ATI.Pharmacy.Web.Controllers
{
    [Area("Pharmacy")]
    [AbpMvcAuthorize(AppPermissions.Pages_Prescriptions)]
    public class PrescriptionsController : ATIControllerBase
    {
        private readonly IPrescriptionsAppService _prescriptionsAppService;
        private readonly IMadicationsAppService _madicationsAppService;
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<Doctor> _doctorRepository;
        private readonly IRepository<Prescription> _prescriptionRepository;
        private readonly IPatientsAppService _patientsAppService;
        private readonly IAllergyAppService _allergyAppService;
        private readonly IAddressAppService _addressAppServce;
        private readonly IDoctorsAppService _doctorsAppService;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IUserCompanyAppService _userCompanyAppService;
        private readonly UserManager _userManager;
        public PrescriptionsController(IPrescriptionsAppService prescriptionsService,
            IMadicationsAppService madicationsAppService,
            IRepository<Patient> patientRepository,
            IRepository<Doctor> doctorRepository,
            IRepository<Prescription> presriptionRepository,
            IPatientsAppService patientsAppService,
            IAllergyAppService allergyAppService,
            IAddressAppService addressAppServce,
            IDoctorsAppService odctorsAppService,
            IBinaryObjectManager binaryObjectManager,
            IUserCompanyAppService userCompanyAppService,
            UserManager userManager)

        {
            _prescriptionsAppService = prescriptionsService;
            _madicationsAppService = madicationsAppService;
            _patientRepository = patientRepository;
            _prescriptionRepository = presriptionRepository;
            _doctorRepository = doctorRepository;
            _patientsAppService = patientsAppService;
            _allergyAppService = allergyAppService;
            _addressAppServce = addressAppServce;
            _doctorsAppService = odctorsAppService;
            _binaryObjectManager = binaryObjectManager;
            _userManager = userManager;
            _userCompanyAppService = userCompanyAppService;
        }
        public async Task<ActionResult> Index()
        {

            var currentUserId = AbpSession.UserId;
            //get roles
            var user = _userManager.GetUserById(currentUserId ?? 0);
            var roles = await _userManager.GetRolesAsync(user);


            var viewModel = new PrescriptionsViewModel();
            viewModel.PrescriptionStatus = Enum.GetValues(typeof(PrescriptionStatusEnum)).Cast<PrescriptionStatusEnum>()
                .Where(prescriptionStatus =>
                !(roles.Contains("3940adad1759401aab8d8a4b37daec8c") || roles.Contains("292b594325de432ba087f999fb429e36"))
                || (prescriptionStatus != (PrescriptionStatusEnum.Created)))//condition to exclude created status for Pharmacy login
                .Select(e => new SelectListItem
                {
                    Text = e.ToString(),
                    Value = ((int)e).ToString()
                }).ToList();

            //If logged in doctor has more than one facility mapping
            var loggedInUserId = AbpSession.UserId;
          //  var userCompanies = _userCompanyAppService.GetUserCompanies(loggedInUserId);
            var doctor = await _doctorsAppService.GetDoctorByUserId((int)loggedInUserId);
            //if (userCompanies.Count() > 1)
            //{
            //    viewModel.FacilityList = await _userCompanyAppService.UserFacilitySelectList((int)loggedInUserId, true);
            //}

            return View(viewModel);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Prescriptions_Create, AppPermissions.Pages_Prescriptions_Edit)]
        public async Task<PartialViewResult> CreateRefillModal(int? id)
        {
            var newPrescriptionId = _prescriptionsAppService.CopyPrescription(id);
            return await CreateOrEditModal(newPrescriptionId);
        }
        [AbpMvcAuthorize(AppPermissions.Pages_Prescriptions_Create, AppPermissions.Pages_Prescriptions_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(int? id)
        {
            GetPrescriptionForEditOutput getPrescriptionForEditOutput;
            var viewModel = new CreateOrEditPrescriptionModalViewModel();


            //var medicines = await _madicationsAppService.GetMedications(6, 1);
            if (id.HasValue)
            {
                getPrescriptionForEditOutput = await _prescriptionsAppService.GetPrescriptionForEdit(new EntityDto { Id = (int)id });
            }
            else
            {
                getPrescriptionForEditOutput = new GetPrescriptionForEditOutput
                {
                    Prescription = new CreateOrEditPrescriptionDto(),
                    Patient = new CreateOrEditPatientDto(),
                    AllMedicines = new List<MedicationDto>(),

                };
            }

            GetPatientForEditOutput getPatientForEditOutput;


            if (getPrescriptionForEditOutput.Patient.Id.HasValue && getPrescriptionForEditOutput.Patient.Id > 0)
            {
                getPatientForEditOutput = await _patientsAppService.GetPatientForEdit(new EntityDto { Id = (int)getPrescriptionForEditOutput.Patient.Id });
            }
            else
            {
                getPatientForEditOutput = new GetPatientForEditOutput
                {
                    Patient = new CreateOrEditPatientDto(),
                    Address = new AddressDto()
                };
                getPatientForEditOutput.Patient.DateOfBirth = DateTime.Now;

            }
            getPatientForEditOutput.Address.States = await _addressAppServce.SelectAllStates();

            getPatientForEditOutput.Patient.Allergies = await _allergyAppService.GetSelectListItem();
            var patientmodel = new CreateOrEditPatientModalViewModel()
            {
                Patient = getPatientForEditOutput.Patient,
                Address = getPatientForEditOutput.Address,
            };

            var prescriptionId = 0;
            if (id != null) prescriptionId = id.Value;
            viewModel = new CreateOrEditPrescriptionModalViewModel();


            viewModel.DeliveryTypes = Enum.GetValues(typeof(DeliveryTypeEnum))
                        .Cast<DeliveryTypeEnum>()
                        .Select(e => new SelectListItem
                        {
                            Text = L(e.ToString()),
                            Value = ((int)e).ToString(),
                            Selected = (getPrescriptionForEditOutput.Prescription.DeliveryTypeId > 0
                            && getPrescriptionForEditOutput.Prescription.DeliveryTypeId == (int)e)
                            ||
                            (getPrescriptionForEditOutput.Prescription.DeliveryTypeId == 0 &&
                            getPrescriptionForEditOutput.Prescription.DoctorID > 0)

                        }).ToList();

            viewModel.BillingTypes = Enum.GetValues(typeof(BillToEnum))
                        .Cast<BillToEnum>()
                        .Select(e => new SelectListItem
                        {
                            Text = L(e.ToString()),
                            Value = ((int)e).ToString(),
                            Selected = (getPrescriptionForEditOutput.Prescription.BillingTo > 0
                            && getPrescriptionForEditOutput.Prescription.BillingTo == (int)e)
                            ||
                            (getPrescriptionForEditOutput.Prescription.BillingTo == 0 &&
                            getPrescriptionForEditOutput.Prescription.DoctorID > 0)

                        }).ToList();


            var query = await _doctorRepository.GetAllAsync();
            var doctor = query.Where(a => a.UserId == (int)AbpSession.UserId.Value).FirstOrDefault();
            var facilities = await _userCompanyAppService.UserFacilitySelectList((int)AbpSession.UserId.Value);
            if (facilities.Count > 2)
            {
                viewModel.DoctorDefaultFacilityId = int.Parse((facilities.FirstOrDefault(a => a.Selected) ?? facilities.FirstOrDefault(a => a.Value != ""))?.Value ?? "");
                viewModel.DoctorDefaultFacilityName = (facilities.FirstOrDefault(a => a.Selected) ?? facilities.FirstOrDefault(a => a.Value != ""))?.Text ?? "";
            }

            viewModel.Prescription = getPrescriptionForEditOutput.Prescription;
            viewModel.PatientId = getPrescriptionForEditOutput.Patient?.Id;
            viewModel.Patient = patientmodel;
            viewModel.AllMedicines = await _madicationsAppService.GetMedications(prescriptionId, (int)MedicationCategoryEnum.Medication);
            viewModel.NauseaMedications = await _madicationsAppService.GetMedications(prescriptionId, (int)MedicationCategoryEnum.Nausea);
            viewModel.OtherMedications = await _madicationsAppService.GetMedications(prescriptionId, (int)MedicationCategoryEnum.Other);
            viewModel.Doctors = await _doctorsAppService.GetAllPharmacyDoctor();
            viewModel.DoctorId = getPrescriptionForEditOutput.Prescription.DoctorID;


            if (viewModel.Doctors.Count < 3 && viewModel.Doctors.Count > 0 && viewModel.DoctorId == 0)
            {
                viewModel.Doctors = viewModel.Doctors.Select(a => new SelectListItem { Selected = a.Value != "", Value = a.Value, Text = a.Text }).ToList();
                viewModel.DoctorId = Convert.ToInt32(viewModel.Doctors.FirstOrDefault(a => a.Selected)?.Value);
            }
            viewModel.DosageRoutes = await _madicationsAppService.GetDosageRoutes();


            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> GetMedicationList(int? id, int? prescriptionId)
        {
            var viewModel = new CreateOrEditPrescriptionModalViewModel();
            var prescriptionForEdit = await _prescriptionsAppService.GetPrescriptionForEdit(new EntityDto() { Id = prescriptionId ?? 0 });


            viewModel = new CreateOrEditPrescriptionModalViewModel()
            {
                AllMedicines = await _madicationsAppService.GetMedications(id.Value, (int)MedicationCategoryEnum.Medication),
                NauseaMedications = await _madicationsAppService.GetMedications(id.Value, (int)MedicationCategoryEnum.Nausea),
                OtherMedications = await _madicationsAppService.GetMedications(id.Value, (int)MedicationCategoryEnum.Other),
            };


            if (prescriptionForEdit != null && prescriptionForEdit.Prescription != null && prescriptionForEdit.Prescription.PrescriptionItems.Count > 0)
            {
                foreach (var item in prescriptionForEdit.Prescription.PrescriptionItems)
                {
                    if (!viewModel.SelectedMedicineIds.ContainsKey(item.MedicationId))
                        viewModel.SelectedMedicineIds.Add(item.MedicationId, item.RefillsAllowed);
                }
            }

            return PartialView("_PrescriptionItems", viewModel);
        }
        public async Task<PartialViewResult> ViewPrescriptionModal(int? id)
        {
            GetPrescriptionForViewDto getPrescriptionForView;
            var viewModel = new ViewAndPrintPrescriptionModalViewModel();

            //Get All Medicines         
                if (id.HasValue)
                {
                    getPrescriptionForView = await _prescriptionsAppService.GetPrescriptionForView((int)id);
                    //getPrescriptionForEditOutput.AllMedicines = medicines;
                }
                else
                {
                    getPrescriptionForView = new GetPrescriptionForViewDto
                    {
                        Prescription = new PrescriptionDto(),
                        Patient = new PatientDto()
                    };
                }

                viewModel = new ViewAndPrintPrescriptionModalViewModel()
                {
                    PrescriptionForViewDto = getPrescriptionForView
                };

                if (getPrescriptionForView.Patient.Address != null)
                    getPrescriptionForView.Patient.Address.States = await _addressAppServce.SelectAllStates();


                if (!string.IsNullOrEmpty(getPrescriptionForView.Doctor?.SignatureFileID))
                {
                    //Get Bytes
                    getPrescriptionForView.SignatureBytes = await _doctorsAppService.GetSignature(getPrescriptionForView.Prescription?.PrescriberSignatureFileID ?? "");
                }
           



            return PartialView("_ViewAndPrintModal", viewModel);
        }

        public async Task<PartialViewResult> ViewPrescription(int? id)
        {
            GetPrescriptionForViewDto getPrescriptionForView = new GetPrescriptionForViewDto();
            //var viewModel = new ViewAndPrintPrescriptionModalViewModel();

            //Get All Medicines
            
                if (id.HasValue)
                {
                    getPrescriptionForView = await _prescriptionsAppService.GetPrescriptionForView((int)id);
                    //getPrescriptionForEditOutput.AllMedicines = medicines;
                }
                else
                {
                    getPrescriptionForView = new GetPrescriptionForViewDto
                    {
                        Prescription = new PrescriptionDto(),
                        Patient = new PatientDto()
                    };
                }

                getPrescriptionForView.Patient.Address.States = await _addressAppServce.SelectAllStates();

           
           

            if (!string.IsNullOrEmpty(getPrescriptionForView.Doctor?.SignatureFileID))
            {
                //Get Bytes
                getPrescriptionForView.SignatureBytes = await _doctorsAppService.GetSignature(getPrescriptionForView.Prescription?.PrescriberSignatureFileID ?? "");
            }

            return PartialView("_ViewAndPrint", getPrescriptionForView);
        }
        public async Task<IActionResult> Download(int id)
        {
            var prescription = await _prescriptionRepository.FirstOrDefaultAsync(a => a.Id == id);

            var obj = await _binaryObjectManager.GetOrNullAsync(Guid.Parse(prescription.BinaryFileId));

            return File(obj.Bytes, "application/pdf", $"Prescription_{id}.pdf");
        }

    }
}

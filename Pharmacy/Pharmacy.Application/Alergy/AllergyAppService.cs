using Abp.Domain.Repositories;
using Abp.Web.Mvc.Alerts;
using ATI.Pharmacy.Application.Alergy.Dtos;
using ATI.Pharmacy.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATI.Pharmacy.Application.Alergy
{
    public partial class AllergyAppService : ATIAppServiceBase, IAllergyAppService
    {

        private readonly IRepository<Allergy> _allergyRepository;
        private readonly IRepository<PatientAllergy> _patientAllergy;
        public AllergyAppService(IRepository<Allergy> allergyRepository, IRepository<PatientAllergy> patientAllergy)
        {
            _allergyRepository = allergyRepository;
            _patientAllergy = patientAllergy;
        }

        public Task<List<AllergyDto>> GetAllergyByPatient()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<List<SelectListItem>> GetSelectListItem()
        {
            var allergies = await _allergyRepository.GetAllAsync();
            var result = new List<SelectListItem>();
            //result.Add(new SelectListItem() { Text = "Select Allergies", Value = "0" });
            result.AddRange(allergies.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),   // The value of the select list item (typically ID)
                Text = c.Description           // The displayed text of the select list item (typically Name)
            }).ToList());

            return result;

        }
    }
}

using Abp.Application.Services;
using ATI.Pharmacy.Application.Alergy.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATI.Pharmacy.Application.Alergy
{
    public interface IAllergyAppService: IApplicationService
    {
        Task<List<SelectListItem>> GetSelectListItem();

        Task<List<AllergyDto>> GetAllergyByPatient();
        
    }
}

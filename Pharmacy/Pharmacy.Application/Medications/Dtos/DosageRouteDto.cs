using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATI.Pharmacy.Application.Medications.Dtos
{
    public class DosageRouteDto:EntityDto<int>
    {
        public string Description { get; set; }
        public string Abbreviation { get; set; }

    }
}

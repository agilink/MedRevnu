using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATI.Pharmacy.Application
{
    public class InsuranceDto : EntityDto<int?>
    {
        
        public string InsuranceProvider { get; set; }

        [Required]
        public string PolicyNumber { get; set; }

        public string CoverageDetails { get; set; }
    }
}

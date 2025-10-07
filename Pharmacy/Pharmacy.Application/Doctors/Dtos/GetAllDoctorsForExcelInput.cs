using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using ATI.DataExporting;

namespace ATI.Pharmacy.Dtos;

public class GetAllDoctorsForExcelInput : IExcelColumnSelectionInput
{
    public string Filter { get; set; }
    public List<string> SelectedColumns { get; set; }
    public int? MaxDoctorIDFilter { get; set; }
    public int? MinDoctorIDFilter { get; set; }

    public string SpecialtyFilter { get; set; }

    public string LicenseNumberFilter { get; set; }

}
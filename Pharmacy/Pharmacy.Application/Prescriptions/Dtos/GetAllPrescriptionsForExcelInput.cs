using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using ATI.DataExporting;

namespace ATI.Pharmacy.Dtos;

public class GetAllPrescriptionsForExcelInput : IExcelColumnSelectionInput
{
    public string Filter { get; set; }
    public List<string> SelectedColumns { get; set; }
    public int? MaxPrescriptionIDFilter { get; set; }
    public int? MinPrescriptionIDFilter { get; set; }

    public string SpecialtyFilter { get; set; }

    public string LicenseNumberFilter { get; set; }

}
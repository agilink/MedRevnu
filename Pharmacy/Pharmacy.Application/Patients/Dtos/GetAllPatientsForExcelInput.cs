using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using ATI.DataExporting;

namespace ATI.Pharmacy.Dtos;

public class GetAllPatientsForExcelInput : IExcelColumnSelectionInput
{
    public string? Filter { get; set; } = string.Empty; // Default to empty string
    public List<string> SelectedColumns { get; set; } = new List<string>();

}
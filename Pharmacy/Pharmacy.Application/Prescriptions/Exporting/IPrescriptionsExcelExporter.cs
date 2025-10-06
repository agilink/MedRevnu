using System.Collections.Generic;

using ATI.Pharmacy.Dtos;
using ATI.Dto;

namespace ATI.Pharmacy.Exporting;

public interface IPrescriptionsExcelExporter
{
    FileDto ExportToFile(List<GetPrescriptionForViewDto> Prescriptions, List<string> selectedColumns);
}
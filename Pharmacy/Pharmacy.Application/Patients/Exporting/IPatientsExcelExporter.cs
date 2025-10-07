using System.Collections.Generic;

using ATI.Pharmacy.Dtos;
using ATI.Dto;

namespace ATI.Pharmacy.Exporting;

public interface IPatientsExcelExporter
{
    FileDto ExportToFile(List<GetPatientForViewDto> patients, List<string> selectedColumns);
}
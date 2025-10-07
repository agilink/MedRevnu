using System.Collections.Generic;

using ATI.Pharmacy.Dtos;
using ATI.Dto;

namespace ATI.Pharmacy.Exporting;

public interface IDoctorsExcelExporter
{
    FileDto ExportToFile(List<GetDoctorForViewDto> doctors, List<string> selectedColumns);
}
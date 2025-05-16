using System.Collections.Generic;
using Abp.Dependency;
using ATI.Dto;

namespace ATI.DataImporting.Excel;

public interface IExcelInvalidEntityExporter<TEntityDto> : ITransientDependency
{
    FileDto ExportToFile(List<TEntityDto> entities);
}
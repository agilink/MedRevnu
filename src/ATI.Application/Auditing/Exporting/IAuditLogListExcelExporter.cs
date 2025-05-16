using ATI.Auditing.Dto;
using ATI.Dto;
using ATI.EntityChanges.Dto;
using System.Collections.Generic;

namespace ATI.Auditing.Exporting
{
    public interface IAuditLogListExcelExporter
    {
        FileDto ExportToFile(List<AuditLogListDto> auditLogListDtos);

        FileDto ExportToFile(List<EntityChangeListDto> entityChangeListDtos);
    }
}

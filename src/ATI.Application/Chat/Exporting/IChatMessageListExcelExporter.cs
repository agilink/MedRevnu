using System.Collections.Generic;
using Abp;
using ATI.Chat.Dto;
using ATI.Dto;

namespace ATI.Chat.Exporting
{
    public interface IChatMessageListExcelExporter
    {
        FileDto ExportToFile(UserIdentifier user, List<ChatMessageExportDto> messages);
    }
}

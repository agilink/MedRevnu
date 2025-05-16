using System.Collections.Generic;
using ATI.Authorization.Users.Dto;
using ATI.Dto;

namespace ATI.Authorization.Users.Exporting
{
    public interface IUserListExcelExporter
    {
        FileDto ExportToFile(List<UserListDto> userListDtos, List<string> selectedColumns);
    }
}
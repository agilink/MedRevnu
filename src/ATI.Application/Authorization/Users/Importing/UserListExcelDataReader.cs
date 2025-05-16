using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Abp.Localization;
using ATI.Authorization.Users.Importing.Dto;
using System.Linq;
using Abp.Collections.Extensions;
using ATI.DataExporting.Excel.MiniExcel;
using ATI.DataImporting.Excel;

namespace ATI.Authorization.Users.Importing
{
    public class UserListExcelDataReader(ILocalizationManager localizationManager)
        : MiniExcelExcelImporterBase<ImportUserDto>(localizationManager), IExcelDataReader<ImportUserDto>
    {
        public List<ImportUserDto> GetEntitiesFromExcel(byte[] fileBytes)
        {
            return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }

        private ImportUserDto ProcessExcelRow(dynamic row)
        {
            if (IsRowEmpty(row))
            {
                return null;
            }

            var exceptionMessage = new StringBuilder();
            var user = new ImportUserDto();

            try
            {
                user.UserName = GetRequiredValueFromRowOrNull(row, nameof(user.UserName), exceptionMessage);
                user.Name = GetRequiredValueFromRowOrNull(row,  nameof(user.Name), exceptionMessage);
                user.Surname = GetRequiredValueFromRowOrNull(row, nameof(user.Surname), exceptionMessage);
                user.EmailAddress = GetRequiredValueFromRowOrNull(row, nameof(user.EmailAddress), exceptionMessage);
                user.PhoneNumber = GetOptionalValueFromRowOrNull<string>(row, nameof(user.PhoneNumber), exceptionMessage);
                user.Password = GetRequiredValueFromRowOrNull(row, nameof(user.Password), exceptionMessage);
                user.Roles = GetAssignedRoleNamesFromRow(row);
            }
            catch (Exception exception)
            {
                user.Exception = exception.Message;
            }

            return user;
        }
        
        private string[] GetAssignedRoleNamesFromRow(dynamic row)
        {
            var cellValue = (row as ExpandoObject).GetOrDefault(nameof(ImportUserDto.Roles))?.ToString();
            if (cellValue == null || string.IsNullOrWhiteSpace(cellValue))
            {
                return Array.Empty<string>();
            }

            var roles = cellValue.Split(',');
            return roles.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim())
                .ToArray();
        }
        
        private bool IsRowEmpty(dynamic row)
        {
            var username = (row as ExpandoObject).GetOrDefault(nameof(User.UserName))?.ToString();
            return string.IsNullOrWhiteSpace(username);
        }
    }
}
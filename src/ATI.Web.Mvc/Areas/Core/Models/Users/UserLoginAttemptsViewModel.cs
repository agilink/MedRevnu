using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace ATI.Web.Areas.Core.Models.Users
{
    public class UserLoginAttemptsViewModel
    {
        public List<ComboboxItemDto> LoginAttemptResults { get; set; }
    }
}
using Abp.AutoMapper;
using ATI.Authorization.Users;
using ATI.Authorization.Users.Dto;
using ATI.Web.Areas.Core.Models.Common;

namespace ATI.Web.Areas.Core.Models.Users
{
    [AutoMapFrom(typeof(GetUserPermissionsForEditOutput))]
    public class UserPermissionsEditViewModel : GetUserPermissionsForEditOutput, IPermissionsEditViewModel
    {
        public User User { get; set; }
    }
}
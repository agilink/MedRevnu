using System.Linq;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using ATI.Authorization.Users.Dto;
using ATI.Security;
using ATI.Web.Areas.Core.Models.Common;

namespace ATI.Web.Areas.Core.Models.Users
{
    [AutoMapFrom(typeof(GetUserForEditOutput))]
    public class CreateOrEditUserModalViewModel : GetUserForEditOutput, IOrganizationUnitsEditViewModel
    {
        public bool CanChangeUserName => User.UserName != AbpUserBase.AdminUserName;

        public int AssignedRoleCount
        {
            get { return Roles.Count(r => r.IsAssigned); }
        }
        
        public int AssignedOrganizationUnitCount => MemberedOrganizationUnits.Count;

        public bool IsEditMode => User.Id.HasValue;

        public PasswordComplexitySetting PasswordComplexitySetting { get; set; }
    }
}
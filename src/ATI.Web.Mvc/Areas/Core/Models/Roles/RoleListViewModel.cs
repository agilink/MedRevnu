using System.Collections.Generic;
using Abp.Application.Services.Dto;
using ATI.Authorization.Permissions.Dto;
using ATI.Web.Areas.Core.Models.Common;

namespace ATI.Web.Areas.Core.Models.Roles
{
    public class RoleListViewModel : IPermissionsEditViewModel
    {
        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}
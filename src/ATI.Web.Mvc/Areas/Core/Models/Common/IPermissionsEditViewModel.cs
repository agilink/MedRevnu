using System.Collections.Generic;
using ATI.Authorization.Permissions.Dto;

namespace ATI.Web.Areas.Core.Models.Common
{
    public interface IPermissionsEditViewModel
    {
        List<FlatPermissionDto> Permissions { get; set; }

        List<string> GrantedPermissionNames { get; set; }
    }
}
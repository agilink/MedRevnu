using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ATI.Authorization.Permissions.Dto;

namespace ATI.Web.Areas.Core.Models.Common.Modals
{
    public class PermissionTreeModalViewModel : IPermissionsEditViewModel
    {
        public List<FlatPermissionDto> Permissions { get; set; }
        public List<string> GrantedPermissionNames { get; set; }
    }
}

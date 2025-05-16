using System.Collections.Generic;
using ATI.Authorization.Delegation;
using ATI.Authorization.Users.Delegation.Dto;

namespace ATI.Web.Areas.Core.Models.Layout
{
    public class ActiveUserDelegationsComboboxViewModel
    {
        public IUserDelegationConfiguration UserDelegationConfiguration { get; set; }

        public List<UserDelegationDto> UserDelegations { get; set; }

        public string CssClass { get; set; }
    }
}

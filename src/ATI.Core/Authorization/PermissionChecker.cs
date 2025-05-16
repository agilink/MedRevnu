using Abp.Authorization;
using ATI.Authorization.Roles;
using ATI.Authorization.Users;

namespace ATI.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}

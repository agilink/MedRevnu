using Abp.AspNetCore.OpenIddict.Claims;
using Abp.AspNetCore.OpenIddict.Controllers;
using Abp.Authorization;
using Abp.Authorization.Users;
using ATI.Authorization.Roles;
using ATI.Authorization.Users;
using ATI.MultiTenancy;
using OpenIddict.Abstractions;

namespace ATI.Web.OpenIddict.Controllers
{
    public class AuthorizeController : AuthorizeController<Tenant, Role, User>
    {
        public AuthorizeController(AbpSignInManager<Tenant, Role, User> signInManager,
            AbpUserManager<Role, User> userManager, IOpenIddictApplicationManager applicationManager,
            IOpenIddictAuthorizationManager authorizationManager, IOpenIddictScopeManager scopeManager,
            IOpenIddictTokenManager tokenManager,
            AbpOpenIddictClaimsPrincipalManager openIddictClaimsPrincipalManager) : base(signInManager, userManager,
            applicationManager, authorizationManager, scopeManager, tokenManager, openIddictClaimsPrincipalManager)
        {
        }
    }
}
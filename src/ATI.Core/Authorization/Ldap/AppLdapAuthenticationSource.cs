using Abp.Zero.Ldap.Authentication;
using Abp.Zero.Ldap.Configuration;
using ATI.Authorization.Users;
using ATI.MultiTenancy;

namespace ATI.Authorization.Ldap
{
    public class AppLdapAuthenticationSource : LdapAuthenticationSource<Tenant, User>
    {
        public AppLdapAuthenticationSource(ILdapSettings settings, IAbpZeroLdapModuleConfig ldapModuleConfig)
            : base(settings, ldapModuleConfig)
        {
        }
    }
}
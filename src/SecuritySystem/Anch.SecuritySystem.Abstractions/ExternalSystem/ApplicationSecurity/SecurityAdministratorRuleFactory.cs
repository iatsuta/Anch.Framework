using Anch.Core;

namespace Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;

public class SecurityAdministratorRuleFactory(SecurityAdministratorRuleInfo securityAdministratorRuleInfo)
    : IFactory<DomainSecurityRule.RoleBaseSecurityRule>
{
    public DomainSecurityRule.RoleBaseSecurityRule Create() => securityAdministratorRuleInfo.SecurityRole;
}

using Anch.Core;

namespace Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;

/// <summary>Supplies the configured security administrator rule.</summary>
/// <param name="securityAdministratorRuleInfo">The configured rule.</param>
public class SecurityAdministratorRuleFactory(SecurityAdministratorRuleInfo securityAdministratorRuleInfo)
    : IFactory<DomainSecurityRule.RoleBaseSecurityRule>
{
    /// <inheritdoc/>
    public DomainSecurityRule.RoleBaseSecurityRule Create() => securityAdministratorRuleInfo.SecurityRole;
}

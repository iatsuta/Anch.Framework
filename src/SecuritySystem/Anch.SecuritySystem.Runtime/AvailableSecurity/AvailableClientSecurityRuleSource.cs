using Anch.SecuritySystem.SecurityRuleInfo;

namespace Anch.SecuritySystem.AvailableSecurity;

/// <summary>Lists the client rules satisfied by the roles that the current user has.</summary>
public class AvailableClientSecurityRuleSource(
    IAvailableSecurityRoleSource availableSecurityRoleSource,
    IClientSecurityRuleResolver clientSecurityRuleResolver) : IAvailableClientSecurityRuleSource
{
    public IAsyncEnumerable<DomainSecurityRule.ClientSecurityRule> GetAvailableSecurityRules() =>
        availableSecurityRoleSource
            .GetAvailableSecurityRoles()
            .SelectMany(securityRole => clientSecurityRuleResolver.Resolve(securityRole))
            .Distinct();
}
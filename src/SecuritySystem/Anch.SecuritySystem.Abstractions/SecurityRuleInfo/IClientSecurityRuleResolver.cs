using System.Collections.Immutable;

namespace Anch.SecuritySystem.SecurityRuleInfo;

/// <summary>Finds the client rules that a role takes part in.</summary>
public interface IClientSecurityRuleResolver
{
    /// <summary>Returns the client rules that the role satisfies.</summary>
    /// <param name="securityRole">The role.</param>
    ImmutableArray<DomainSecurityRule.ClientSecurityRule> Resolve(SecurityRole securityRole);
}

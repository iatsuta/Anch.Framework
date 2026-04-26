using System.Collections.Immutable;

namespace Anch.SecuritySystem.SecurityRuleInfo;

public interface IClientSecurityRuleResolver
{
    ImmutableArray<DomainSecurityRule.ClientSecurityRule> Resolve(SecurityRole securityRole);
}

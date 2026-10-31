using System.Collections.Frozen;

using Anch.Core;
using Anch.SecuritySystem.SecurityRuleInfo;

namespace Anch.SecuritySystem.Expanders;

/// <summary>Replaces an access mode of a domain type with the rule configured for it.</summary>
public class SecurityModeExpander(IEnumerable<DomainModeSecurityRuleInfo> infoList) : ISecurityModeExpander
{
    private readonly FrozenDictionary<DomainSecurityRule.DomainModeSecurityRule, DomainSecurityRule> dict =
        infoList.ToFrozenDictionary(info => info.SecurityRule, info => info.Implementation);

    public DomainSecurityRule? TryExpand(DomainSecurityRule.DomainModeSecurityRule securityRule)
    {
        return this.dict.GetValueOrDefault(securityRule.WithDefaultCredential()).Maybe(v => v with { CustomCredential = securityRule.CustomCredential });
    }
}
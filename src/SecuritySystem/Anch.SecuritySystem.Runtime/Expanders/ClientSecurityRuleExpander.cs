using System.Collections.Frozen;
using Anch.SecuritySystem.SecurityRuleInfo;

namespace Anch.SecuritySystem.Expanders;

public class ClientSecurityRuleExpander(IClientSecurityRuleInfoSource clientSecurityRuleInfoSource) : IClientSecurityRuleExpander
{
    private readonly FrozenDictionary<DomainSecurityRule.ClientSecurityRule, DomainSecurityRule> dict =
        clientSecurityRuleInfoSource.GetInfos().ToFrozenDictionary(info => info.Rule, info => info.Implementation);

    public DomainSecurityRule Expand(DomainSecurityRule.ClientSecurityRule baseSecurityRule)
    {
        return baseSecurityRule.WithDefaultCredential(securityRule =>
            this.dict.GetValueOrDefault(securityRule) ?? throw new ArgumentOutOfRangeException(nameof(securityRule),
                $"{nameof(DomainSecurityRule.ClientSecurityRule)} with name \"{securityRule.Name}\" not found"));
    }
}
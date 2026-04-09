using System.Collections.Frozen;

using CommonFramework;

using SecuritySystem.SecurityRuleInfo;

namespace SecuritySystem.Expanders;

public class SecurityRuleHeaderExpander(IEnumerable<SecurityRuleHeaderInfo> securityRuleFullInfoList) : ISecurityRuleHeaderExpander
{
    private readonly FrozenDictionary<DomainSecurityRule.SecurityRuleHeader, DomainSecurityRule> dict =
        securityRuleFullInfoList.ToFrozenDictionary(pair => pair.Header, pair => pair.Implementation);

    public DomainSecurityRule Expand(DomainSecurityRule.SecurityRuleHeader baseSecurityRule) =>
        baseSecurityRule.WithDefaultCredential(securityRuleHeader =>
            this.dict.GetValue(
                securityRuleHeader,
                () => new ArgumentOutOfRangeException(nameof(securityRuleHeader),
                    $"Implementation for {nameof(SecurityRule)} \"{securityRuleHeader}\" not found")));
}
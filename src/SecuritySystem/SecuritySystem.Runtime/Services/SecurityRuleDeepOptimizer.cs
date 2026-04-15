using System.Collections.Concurrent;
using SecuritySystem.Expanders;

namespace SecuritySystem.Services;

public class SecurityRuleDeepOptimizer(
    ISecurityRuleExpander expander,
    ISecurityRuleBasicOptimizer basicOptimizer)
    : ISecurityRuleDeepOptimizer
{
    private readonly ConcurrentDictionary<DomainSecurityRule, DomainSecurityRule> cache = [];

    protected virtual DomainSecurityRule Visit(DomainSecurityRule baseSecurityRule)
    {
        var visitedRule = basicOptimizer.Optimize(expander.FullDomainExpand(baseSecurityRule));

        return visitedRule == baseSecurityRule ? visitedRule : this.Visit(visitedRule);
    }

    DomainSecurityRule ISecurityRuleDeepOptimizer.Optimize(DomainSecurityRule baseSecurityRule) =>

        baseSecurityRule.WithDefaultCredential(securityRule => this.cache.GetOrAdd(securityRule, _ => this.Visit(securityRule)));
}
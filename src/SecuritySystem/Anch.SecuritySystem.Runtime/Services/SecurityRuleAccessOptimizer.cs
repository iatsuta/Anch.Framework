using System.Collections.Concurrent;

using Anch.SecuritySystem.Expanders;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Services;

/// <summary>Prepares a rule for an access check: expands operations, domain modes, headers and client rules down to non-expanded role rules, so that <see cref="SecuritySystem"/> can compare them with the available roles without expanding role groups.</summary>
public class SecurityRuleAccessOptimizer(
    ISecurityRuleExpander expander,
    [FromKeyedServices(ISecurityRuleOptimizer.BasicKey)]
    ISecurityRuleOptimizer basicOptimizer) : ISecurityRuleOptimizer
{
    private readonly AccessExpandVisitor visitor = new (expander);

    private readonly ConcurrentDictionary<DomainSecurityRule, DomainSecurityRule> cache = [];

    protected virtual DomainSecurityRule Visit(DomainSecurityRule baseSecurityRule)
    {
        var visitedRule = basicOptimizer.Optimize(this.visitor.Visit(baseSecurityRule));

        return visitedRule == baseSecurityRule ? visitedRule : this.Visit(visitedRule);
    }

    DomainSecurityRule ISecurityRuleOptimizer.Optimize(DomainSecurityRule baseSecurityRule) =>

        baseSecurityRule.WithDefaultCredential(securityRule => this.cache.GetOrAdd(securityRule, _ => this.Visit(securityRule)));



    private class AccessExpandVisitor(ISecurityRuleExpander expander) : SecurityRuleVisitor
    {
        protected override DomainSecurityRule Visit(DomainSecurityRule.OperationSecurityRule securityRule) => this.Visit(expander.Expand(securityRule));

        protected override DomainSecurityRule Visit(DomainSecurityRule.DomainModeSecurityRule securityRule) => this.Visit(expander.Expand(securityRule));

        protected override DomainSecurityRule Visit(DomainSecurityRule.SecurityRuleHeader securityRule) => this.Visit(expander.Expand(securityRule));

        protected override DomainSecurityRule Visit(DomainSecurityRule.ClientSecurityRule securityRule) => this.Visit(expander.Expand(securityRule));
    }
}
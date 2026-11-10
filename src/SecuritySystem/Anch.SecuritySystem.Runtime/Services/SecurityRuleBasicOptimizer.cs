using System.Collections.Concurrent;

using static Anch.SecuritySystem.DomainSecurityRule;
using static Anch.SecuritySystem.SecurityRule;

namespace Anch.SecuritySystem.Services;

/// <summary>Simplifies a rule without expanding it, for example by merging neighbouring role rules.</summary>
public class SecurityRuleBasicOptimizer : SecurityRuleVisitor, ISecurityRuleOptimizer
{
    private readonly ConcurrentDictionary<DomainSecurityRule, DomainSecurityRule> cache = [];

    protected override DomainSecurityRule Visit(OrSecurityRule baseSecurityRule)
    {
        var visitedBase = base.Visit(baseSecurityRule);

        return visitedBase switch
        {
            OrSecurityRule { Left: { } left, Right: { } right }
                when left == Disabled || right == Disabled => Disabled.TryApply(baseSecurityRule.CustomCredential),

            OrSecurityRule { Left: ExpandedRolesSecurityRule left, Right: ExpandedRolesSecurityRule right }
                when left.EqualsCustoms(right) => (left + right).TryApply(baseSecurityRule.CustomCredential),

            OrSecurityRule
            {
                Left: OrSecurityRule { Left: ExpandedRolesSecurityRule leftLeft, Right: { } leftRight },
                Right: ExpandedRolesSecurityRule right
            } when leftLeft.EqualsCustoms(right) => leftRight.Or(leftLeft + right).TryApply(baseSecurityRule.CustomCredential),

            OrSecurityRule
            {
                Left: OrSecurityRule { Left: { } leftLeft, Right: ExpandedRolesSecurityRule leftRight },
                Right: ExpandedRolesSecurityRule right
            } when leftRight.EqualsCustoms(right) => leftLeft.Or(leftRight + right).TryApply(baseSecurityRule.CustomCredential),

            OrSecurityRule
            {
                Left: ExpandedRolesSecurityRule left,
                Right: OrSecurityRule { Left: ExpandedRolesSecurityRule rightLeft, Right: { } rightRight }
            } when left.EqualsCustoms(rightLeft) => (left + rightLeft).Or(rightRight).TryApply(baseSecurityRule.CustomCredential),

            OrSecurityRule
            {
                Left: ExpandedRolesSecurityRule left,
                Right: OrSecurityRule { Left: { } rightLeft, Right: ExpandedRolesSecurityRule rightRight }
            } when left.EqualsCustoms(rightRight) => (left + rightRight).Or(rightLeft).TryApply(baseSecurityRule.CustomCredential),

            //NonExpandedRolesSecurityRule

            OrSecurityRule { Left: NonExpandedRolesSecurityRule left, Right: NonExpandedRolesSecurityRule right }
                when left.EqualsCustoms(right) => (left + right).TryApply(baseSecurityRule.CustomCredential),

            OrSecurityRule
            {
                Left: OrSecurityRule { Left: NonExpandedRolesSecurityRule leftLeft, Right: { } leftRight },
                Right: NonExpandedRolesSecurityRule right
            } when leftLeft.EqualsCustoms(right) => leftRight.Or(leftLeft + right).TryApply(baseSecurityRule.CustomCredential),

            OrSecurityRule
            {
                Left: OrSecurityRule { Left: { } leftLeft, Right: NonExpandedRolesSecurityRule leftRight },
                Right: NonExpandedRolesSecurityRule right
            } when leftRight.EqualsCustoms(right) => leftLeft.Or(leftRight + right).TryApply(baseSecurityRule.CustomCredential),

            OrSecurityRule
            {
                Left: NonExpandedRolesSecurityRule left,
                Right: OrSecurityRule { Left: NonExpandedRolesSecurityRule rightLeft, Right: { } rightRight }
            } when left.EqualsCustoms(rightLeft) => (left + rightLeft).Or(rightRight).TryApply(baseSecurityRule.CustomCredential),

            OrSecurityRule
            {
                Left: NonExpandedRolesSecurityRule left,
                Right: OrSecurityRule { Left: { } rightLeft, Right: NonExpandedRolesSecurityRule rightRight }
            } when left.EqualsCustoms(rightRight) => (left + rightRight).Or(rightLeft).TryApply(baseSecurityRule.CustomCredential),

            _ => visitedBase
        };
    }

    protected override DomainSecurityRule Visit(AndSecurityRule baseSecurityRule)
    {
        var visitedBase = base.Visit(baseSecurityRule);

        return visitedBase switch
        {
            AndSecurityRule { Left: { } left, Right: { } right } when left == Disabled => right.TryApply(baseSecurityRule.CustomCredential),

            AndSecurityRule { Left: { } left, Right: { } right } when right == Disabled => left.TryApply(baseSecurityRule.CustomCredential),

            _ => visitedBase
        };
    }

    protected override DomainSecurityRule Visit(NegateSecurityRule baseSecurityRule)
    {
        var visitedBase = base.Visit(baseSecurityRule);

        return visitedBase switch
        {
            NegateSecurityRule { InnerRule: NegateSecurityRule deepNegateRule } => deepNegateRule.InnerRule.TryApply(baseSecurityRule.CustomCredential),

            _ => visitedBase
        };
    }

    DomainSecurityRule ISecurityRuleOptimizer.Optimize(DomainSecurityRule baseSecurityRule) =>
        baseSecurityRule.WithDefaultCredential(securityRule => this.cache.GetOrAdd(securityRule, _ => this.Visit(securityRule)));
}
using System.Linq.Expressions;

using Anch.Core;
using Anch.HierarchicalExpand;

using static Anch.SecuritySystem.DomainSecurityRule;

namespace Anch.SecuritySystem;

/// <summary>
/// Combinators for security rules. They are available on rules, roles, operations and role collections,
/// which are converted to rules automatically.
/// </summary>
public static class SecurityRuleExtensions
{
    extension(DomainSecurityRule securityRule)
    {
        /// <summary>Combines the rules: access is granted if either of them is satisfied.</summary>
        /// <param name="otherSecurityRule">The alternative rule.</param>
        public DomainSecurityRule Or(DomainSecurityRule otherSecurityRule) =>
            new OrSecurityRule(securityRule, otherSecurityRule);

        /// <summary>Combines the rules: access is granted only if both are satisfied.</summary>
        /// <param name="otherSecurityRule">The additional rule.</param>
        public DomainSecurityRule And(DomainSecurityRule otherSecurityRule) =>
            new AndSecurityRule(securityRule, otherSecurityRule);

        /// <summary>Adds an alternative: access is also granted if the condition holds for the related object.</summary>
        /// <param name="condition">Condition on the related object.</param>
        /// <typeparam name="TRelativeDomainObject">The type of the related object.</typeparam>
        public DomainSecurityRule Or<TRelativeDomainObject>(Expression<Func<TRelativeDomainObject, bool>> condition) =>
            securityRule.Or(new RelativeConditionSecurityRule(condition.ToInfo()));

        /// <summary>Adds a requirement: access is granted only if the condition also holds for the related object.</summary>
        /// <param name="condition">Condition on the related object.</param>
        /// <typeparam name="TRelativeDomainObject">The type of the related object.</typeparam>
        public DomainSecurityRule And<TRelativeDomainObject>(Expression<Func<TRelativeDomainObject, bool>> condition) =>
            securityRule.And(new RelativeConditionSecurityRule(condition.ToInfo()));

        /// <summary>Excludes the objects for which the condition holds.</summary>
        /// <param name="condition">Condition on the related object that must not hold.</param>
        /// <typeparam name="TRelativeDomainObject">The type of the related object.</typeparam>
        public DomainSecurityRule Except<TRelativeDomainObject>(Expression<Func<TRelativeDomainObject, bool>> condition) =>
            securityRule.And(condition.Not());

        /// <summary>Inverts the rule: access is granted only if it is not satisfied.</summary>
        public DomainSecurityRule Negate() =>
            new NegateSecurityRule(securityRule);

        /// <summary>Excludes what the other rule grants: satisfied if this rule is and the other is not.</summary>
        /// <param name="otherSecurityRule">The rule to exclude.</param>
        public DomainSecurityRule Except(DomainSecurityRule otherSecurityRule) =>
            securityRule.And(otherSecurityRule.Negate());
    }

    extension(SecurityRole securityRole)
    {
        /// <summary>Combines the rules: access is granted if either of them is satisfied.</summary>
        /// <param name="otherSecurityRule">The alternative rule.</param>
        public DomainSecurityRule Or(DomainSecurityRule otherSecurityRule) =>
            securityRole.ToSecurityRule().Or(otherSecurityRule);

        /// <summary>Combines the rules: access is granted only if both are satisfied.</summary>
        /// <param name="otherSecurityRule">The additional rule.</param>
        public DomainSecurityRule And(DomainSecurityRule otherSecurityRule) =>
            securityRole.ToSecurityRule().And(otherSecurityRule);

        /// <summary>Adds an alternative: access is also granted if the condition holds for the related object.</summary>
        /// <param name="condition">Condition on the related object.</param>
        /// <typeparam name="TRelativeDomainObject">The type of the related object.</typeparam>
        public DomainSecurityRule Or<TRelativeDomainObject>(Expression<Func<TRelativeDomainObject, bool>> condition) =>
            securityRole.ToSecurityRule().Or(condition);

        /// <summary>Adds a requirement: access is granted only if the condition also holds for the related object.</summary>
        /// <param name="condition">Condition on the related object.</param>
        /// <typeparam name="TRelativeDomainObject">The type of the related object.</typeparam>
        public DomainSecurityRule And<TRelativeDomainObject>(Expression<Func<TRelativeDomainObject, bool>> condition) =>
            securityRole.ToSecurityRule().And(condition);

        /// <summary>Excludes the objects for which the condition holds.</summary>
        /// <param name="condition">Condition on the related object that must not hold.</param>
        /// <typeparam name="TRelativeDomainObject">The type of the related object.</typeparam>
        public DomainSecurityRule Except<TRelativeDomainObject>(Expression<Func<TRelativeDomainObject, bool>> condition) =>
            securityRole.ToSecurityRule().Except(condition);

        /// <summary>Inverts the rule: access is granted only if it is not satisfied.</summary>
        public DomainSecurityRule Negate() => securityRole.ToSecurityRule().Negate();

        /// <summary>Excludes what the other rule grants: satisfied if this rule is and the other is not.</summary>
        /// <param name="otherSecurityRule">The rule to exclude.</param>
        public DomainSecurityRule Except(DomainSecurityRule otherSecurityRule) =>
            securityRole.ToSecurityRule().Except(otherSecurityRule);

        /// <summary>Creates a rule that is satisfied by this role.</summary>
        /// <param name="customExpandType">How to expand hierarchical security contexts; <c>null</c> for the default (children).</param>
        /// <param name="customRestriction">Overrides the restrictions of the role security path.</param>
        public NonExpandedRolesSecurityRule ToSecurityRule(
            HierarchicalExpandType? customExpandType = null,
            SecurityPathRestriction? customRestriction = null) =>
            new[] { securityRole }.ToSecurityRule(customExpandType, customRestriction);
    }

    extension(SecurityOperation securityOperation)
    {
        /// <summary>Combines the rules: access is granted if either of them is satisfied.</summary>
        /// <param name="otherSecurityRule">The alternative rule.</param>
        public DomainSecurityRule Or(DomainSecurityRule otherSecurityRule) =>
            securityOperation.ToSecurityRule().Or(otherSecurityRule);

        /// <summary>Combines the rules: access is granted only if both are satisfied.</summary>
        /// <param name="otherSecurityRule">The additional rule.</param>
        public DomainSecurityRule And(DomainSecurityRule otherSecurityRule) =>
            securityOperation.ToSecurityRule().And(otherSecurityRule);

        /// <summary>Adds an alternative: access is also granted if the condition holds for the related object.</summary>
        /// <param name="condition">Condition on the related object.</param>
        /// <typeparam name="TRelativeDomainObject">The type of the related object.</typeparam>
        public DomainSecurityRule Or<TRelativeDomainObject>(Expression<Func<TRelativeDomainObject, bool>> condition) =>
            securityOperation.ToSecurityRule().Or(condition);

        /// <summary>Adds a requirement: access is granted only if the condition also holds for the related object.</summary>
        /// <param name="condition">Condition on the related object.</param>
        /// <typeparam name="TRelativeDomainObject">The type of the related object.</typeparam>
        public DomainSecurityRule And<TRelativeDomainObject>(Expression<Func<TRelativeDomainObject, bool>> condition) =>
            securityOperation.ToSecurityRule().And(condition);

        /// <summary>Excludes the objects for which the condition holds.</summary>
        /// <param name="condition">Condition on the related object that must not hold.</param>
        /// <typeparam name="TRelativeDomainObject">The type of the related object.</typeparam>
        public DomainSecurityRule Except<TRelativeDomainObject>(Expression<Func<TRelativeDomainObject, bool>> condition) =>
            securityOperation.ToSecurityRule().Except(condition);

        /// <summary>Inverts the rule: access is granted only if it is not satisfied.</summary>
        public DomainSecurityRule Negate() =>
            securityOperation.ToSecurityRule().Negate();

        /// <summary>Excludes what the other rule grants: satisfied if this rule is and the other is not.</summary>
        /// <param name="otherSecurityRule">The rule to exclude.</param>
        public DomainSecurityRule Except(DomainSecurityRule otherSecurityRule) =>
            securityOperation.ToSecurityRule().Except(otherSecurityRule);

        /// <summary>Creates a rule that is satisfied by the roles that include this operation.</summary>
        /// <param name="customExpandType">How to expand hierarchical security contexts; <c>null</c> for the default (children).</param>
        /// <param name="customCredential">Whose permissions are checked; <c>null</c> for the current user.</param>
        /// <param name="customRestriction">Overrides the restrictions of the role security path.</param>
        public OperationSecurityRule ToSecurityRule(
            HierarchicalExpandType? customExpandType = null,
            SecurityRuleCredential? customCredential = null,
            SecurityPathRestriction? customRestriction = null) =>
            new(securityOperation)
            {
                CustomExpandType = customExpandType,
                CustomCredential = customCredential,
                CustomRestriction = customRestriction
            };
    }

    extension(IEnumerable<SecurityRole> securityRoles)
    {
        /// <summary>Combines the rules: access is granted if either of them is satisfied.</summary>
        /// <param name="otherSecurityRule">The alternative rule.</param>
        public DomainSecurityRule Or(DomainSecurityRule otherSecurityRule) =>
            securityRoles.ToSecurityRule().Or(otherSecurityRule);

        /// <summary>Combines the rules: access is granted only if both are satisfied.</summary>
        /// <param name="otherSecurityRule">The additional rule.</param>
        public DomainSecurityRule And(DomainSecurityRule otherSecurityRule) =>
            securityRoles.ToSecurityRule().And(otherSecurityRule);

        /// <summary>Adds an alternative: access is also granted if the condition holds for the related object.</summary>
        /// <param name="condition">Condition on the related object.</param>
        /// <typeparam name="TRelativeDomainObject">The type of the related object.</typeparam>
        public DomainSecurityRule Or<TRelativeDomainObject>(Expression<Func<TRelativeDomainObject, bool>> condition) =>
            securityRoles.ToSecurityRule().Or(condition);

        /// <summary>Adds a requirement: access is granted only if the condition also holds for the related object.</summary>
        /// <param name="condition">Condition on the related object.</param>
        /// <typeparam name="TRelativeDomainObject">The type of the related object.</typeparam>
        public DomainSecurityRule And<TRelativeDomainObject>(Expression<Func<TRelativeDomainObject, bool>> condition) =>
            securityRoles.ToSecurityRule().And(condition);

        /// <summary>Excludes the objects for which the condition holds.</summary>
        /// <param name="condition">Condition on the related object that must not hold.</param>
        /// <typeparam name="TRelativeDomainObject">The type of the related object.</typeparam>
        public DomainSecurityRule Except<TRelativeDomainObject>(Expression<Func<TRelativeDomainObject, bool>> condition) =>
            securityRoles.ToSecurityRule().Except(condition);

        /// <summary>Inverts the rule: access is granted only if it is not satisfied.</summary>
        public DomainSecurityRule Negate() =>
            securityRoles.ToSecurityRule().Negate();

        /// <summary>Excludes what the other rule grants: satisfied if this rule is and the other is not.</summary>
        /// <param name="otherSecurityRule">The rule to exclude.</param>
        public DomainSecurityRule Except(DomainSecurityRule otherSecurityRule) =>
            securityRoles.ToSecurityRule().Except(otherSecurityRule);

        /// <summary>Creates a rule that is satisfied by any of these roles.</summary>
        /// <param name="customExpandType">How to expand hierarchical security contexts; <c>null</c> for the default (children).</param>
        /// <param name="customRestriction">Overrides the restrictions of the role security path.</param>
        public NonExpandedRolesSecurityRule ToSecurityRule(
            HierarchicalExpandType? customExpandType = null,
            SecurityPathRestriction? customRestriction = null) =>
            new(
                securityRoles.OrderBy(sr => sr.Name).ToArray())
            {
                CustomExpandType = customExpandType,
                CustomRestriction = customRestriction
            };
    }

    extension<TSecurityRule>(TSecurityRule securityRule)
        where TSecurityRule : RoleBaseSecurityRule
    {
        /// <summary>Returns the rule that ignores an active RunAs session and checks the real current user.</summary>
        public TSecurityRule WithoutRunAs() =>
            securityRule with { CustomCredential = new SecurityRuleCredential.CurrentUserWithoutRunAsCredential() };

        /// <summary>Copies the customization of <paramref name="customData"/> to the rule, without overriding values the rule already has.</summary>
        /// <param name="customData">The source of the customization.</param>
        public TSecurityRule ApplyCustoms(IRoleBaseSecurityRuleCustomData customData) =>

            securityRule.ApplyCustoms(customData.CustomCredential, customData.CustomExpandType, customData.CustomRestriction);

        /// <summary>Applies the customization to the rule, without overriding values the rule already has.</summary>
        /// <param name="customCredential">Whose permissions are checked.</param>
        /// <param name="customExpandType">How to expand hierarchical security contexts.</param>
        /// <param name="customRestriction">Overrides the restrictions of the role security path.</param>
        public TSecurityRule ApplyCustoms(
            SecurityRuleCredential? customCredential = null,
            HierarchicalExpandType? customExpandType = null,
            SecurityPathRestriction? customRestriction = null) =>

            securityRule
                .TryApply(customCredential)
                .TryApply(customExpandType)
                .TryApply(customRestriction);

        /// <summary>Sets the restriction unless the rule already has one.</summary>
        /// <param name="customRestriction">The restriction to apply.</param>
        public TSecurityRule TryApply(SecurityPathRestriction? customRestriction) =>
            customRestriction is null || securityRule.CustomRestriction is not null
                ? securityRule
                : securityRule with { CustomRestriction = customRestriction };

        /// <summary>Sets the expansion type unless the rule already has one.</summary>
        /// <param name="customExpandType">The expansion type to apply.</param>
        public TSecurityRule TryApply(HierarchicalExpandType? customExpandType) =>
            customExpandType is null || securityRule.CustomExpandType is not null
                ? securityRule
                : securityRule with { CustomExpandType = customExpandType };

        /// <summary>Returns the rule with credential, expansion type and restriction reset to their defaults.</summary>
        public TSecurityRule WithDefaultCustoms() =>
            securityRule.CustomCredential is null && securityRule.CustomExpandType is null && securityRule.CustomRestriction is null
                ? securityRule
                : securityRule with { CustomCredential = null, CustomExpandType = null, CustomRestriction = null };
    }


    extension<TSecurityRule>(TSecurityRule securityRule)
        where TSecurityRule : SecurityRule
    {
        /// <summary>Sets the credential unless the rule already has one.</summary>
        /// <param name="customCredential">The credential to apply.</param>
        public TSecurityRule TryApply(SecurityRuleCredential? customCredential) =>
            customCredential is null || securityRule.CustomCredential is not null
                ? securityRule
                : securityRule with { CustomCredential = customCredential };

        /// <summary>Returns the rule with the credential reset to the default (the current user).</summary>
        public TSecurityRule WithDefaultCredential() =>
            securityRule.CustomCredential is null ? securityRule : securityRule with { CustomCredential = null };

        /// <summary>Applies <paramref name="selector"/> to the rule without its credential, then restores the credential on the result.</summary>
        /// <param name="selector">Transformation of the rule.</param>
        /// <typeparam name="TResult">The type of the resulting rule.</typeparam>
        public TResult WithDefaultCredential<TResult>(Func<TSecurityRule, TResult> selector)
            where TResult : SecurityRule =>
            selector(securityRule.WithDefaultCredential()).TryApply(securityRule.CustomCredential);
    }

    /// <summary>Combines role rules into one rule that is satisfied by any of them.</summary>
    /// <param name="securityRules">The rules to combine.</param>
    /// <param name="customCredential">Whose permissions are checked.</param>
    /// <param name="customExpandType">How to expand hierarchical security contexts.</param>
    /// <param name="customRestriction">Overrides the restrictions of the role security path.</param>
    public static RoleBaseSecurityRule ToSecurityRule(
        this IEnumerable<NonExpandedRolesSecurityRule> securityRules,
        SecurityRuleCredential? customCredential = null,
        HierarchicalExpandType? customExpandType = null,
        SecurityPathRestriction? customRestriction = null)
    {
        var cache = securityRules.ToList();

        if (cache.Count == 1)
        {
            return cache.Single().ApplyCustoms(customCredential, customExpandType, customRestriction);
        }
        else
        {
            return new NonExpandedRoleGroupSecurityRule(cache)
            {
                CustomExpandType = customExpandType,
                CustomCredential = customCredential,
                CustomRestriction = customRestriction
            };
        }
    }

    /// <summary>Replaces the message of the access denied error that this rule produces.</summary>
    /// <param name="securityRule">The rule.</param>
    /// <param name="customMessage">The message to report.</param>
    public static DomainSecurityRule WithOverrideAccessDeniedMessage(
        this DomainSecurityRule securityRule,
        string customMessage) =>
        new OverrideAccessDeniedMessageSecurityRule(securityRule, customMessage);
}
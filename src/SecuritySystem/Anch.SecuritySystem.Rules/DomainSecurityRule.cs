using Anch.Core;
using Anch.HierarchicalExpand;

namespace Anch.SecuritySystem;

/// <summary>
/// A <see cref="SecurityRule"/> that can be checked for domain objects. The derived records cover roles, operations, modes,
/// the current user, conditions, custom providers and the combinators <c>And</c>, <c>Or</c> and <c>Negate</c>.
/// </summary>
public abstract record DomainSecurityRule : SecurityRule
{
    /// <summary>
    /// Access rule for domain objects bound to the current user.
    /// </summary>
    public static CurrentUserSecurityRule CurrentUser { get; } = new();

    /// <summary>
    /// Access rule that denies all access.
    /// </summary>
    public static ProviderSecurityRule AccessDenied { get; } = new() { Key = nameof(AccessDenied) };

    /// <summary>
    /// Any role.
    /// </summary>
    public static AnyRoleSecurityRule AnyRole { get; } = new();


    public static implicit operator DomainSecurityRule(SecurityOperation securityOperation) => securityOperation.ToSecurityRule();

    public static implicit operator DomainSecurityRule(SecurityRole securityRole) => securityRole.ToSecurityRule();

    public static implicit operator DomainSecurityRule(SecurityRole[] securityRoles) => securityRoles.ToSecurityRule();

    public static implicit operator DomainSecurityRule(NonExpandedRolesSecurityRule[] securityRules) => securityRules.ToSecurityRule();

    /// <summary>A named rule whose real content is registered with <c>AddSecurityRule</c> and substituted at expansion time.</summary>
    /// <param name="Name">The name of the rule.</param>
    public record SecurityRuleHeader(string Name) : DomainSecurityRule
    {
        public override string ToString() => this.Name;
    }

    /// <summary>A named rule that a client (for example a UI) can request; resolved through the client security rule sources.</summary>
    /// <param name="Name">The name of the rule.</param>
    public record ClientSecurityRule(string Name) : DomainSecurityRule
    {
        public override string ToString() => this.Name;
    }

    /// <summary>An access mode (view or edit) for a particular domain type; the roles are taken from the type's <c>AddDomainSecurity</c> configuration.</summary>
    /// <param name="DomainType">The secured domain type.</param>
    /// <param name="Mode">The access mode.</param>
    public record DomainModeSecurityRule(Type DomainType, ModeSecurityRule Mode) : DomainSecurityRule
    {
        public override string ToString() => $"{this.Mode} ({this.DomainType.Name})";
    }

    /// <summary>Grants access to the objects that belong to the current user (as defined by a relative path registered for the type).</summary>
    public record CurrentUserSecurityRule : DomainSecurityRule
    {
        /// <summary>Key of the relative path to use when several are registered for the type.</summary>
        public string? RelativePathKey { get; init; }

        public override string ToString() => this.RelativePathKey ?? nameof(CurrentUser);
    }

    /// <summary>A rule implemented by a custom <c>ISecurityProvider&lt;T&gt;</c>, resolved by type or by key.</summary>
    public record ProviderSecurityRule : DomainSecurityRule
    {
        /// <summary>The open generic provider type that implements the rule.</summary>
        public Type? GenericSecurityProviderType { get; init; }

        /// <summary>The key under which the provider is registered.</summary>
        public string? Key { get; init; }

        public override string ToString() => this.Key ?? base.ToString();
    }

    /// <summary>A rule whose provider is created by a custom factory.</summary>
    /// <param name="GenericSecurityProviderFactoryType">The open generic factory type.</param>
    public record ProviderFactorySecurityRule(Type GenericSecurityProviderFactoryType) : DomainSecurityRule
    {
        public string? Key { get; init; }

        public override string ToString() => this.Key ?? base.ToString();
    }

    /// <summary>A rule whose filter condition is produced by a custom condition factory.</summary>
    /// <param name="GenericConditionFactoryType">The open generic condition factory type.</param>
    public record ConditionFactorySecurityRule(Type GenericConditionFactoryType) : DomainSecurityRule;

    /// <summary>A rule that checks a condition on a related object.</summary>
    /// <param name="RelativeConditionInfo">The condition.</param>
    public record RelativeConditionSecurityRule(RelativeConditionInfo RelativeConditionInfo) : DomainSecurityRule;

    /// <summary>A rule that is produced by a custom factory at expansion time.</summary>
    /// <param name="RuleFactoryType">The factory type.</param>
    public record FactorySecurityRule(Type RuleFactoryType) : DomainSecurityRule;

    /// <summary>Wraps a rule and replaces the message of the access denied error.</summary>
    /// <param name="BaseSecurityRule">The wrapped rule.</param>
    /// <param name="CustomMessage">The message to report instead of the default one.</param>
    public record OverrideAccessDeniedMessageSecurityRule(DomainSecurityRule BaseSecurityRule, string CustomMessage) : DomainSecurityRule;

    /// <summary>A rule that is satisfied by roles. Allows customizing how security contexts are expanded, which restrictions apply and whose permissions are checked.</summary>
    public abstract record RoleBaseSecurityRule : DomainSecurityRule, IRoleBaseSecurityRuleCustomData
    {
        /// <summary>
        /// Tree expansion type (usually HierarchicalExpandType.All is chosen to view the tree itself).
        /// When null, <see cref="HierarchicalExpandType.Children"/> is used.
        /// </summary>
        public HierarchicalExpandType? CustomExpandType { get; init; } = null;

        /// <summary>Overrides the restrictions of the roles' security path; <c>null</c> keeps the role's own restrictions.</summary>
        public SecurityPathRestriction? CustomRestriction { get; init; } = null;

        /// <summary>The effective expansion type: the custom one or <see cref="HierarchicalExpandType.Children"/>.</summary>
        public HierarchicalExpandType GetSafeExpandType() => this.CustomExpandType ?? HierarchicalExpandType.Children;

        /// <summary>The custom context restrictions, or an empty sequence.</summary>
        public IEnumerable<SecurityContextRestriction> GetSafeSecurityContextRestrictions() =>
            (this.CustomRestriction?.SecurityContextRestrictions).EmptyIfNull();

        /// <summary>The filters of the custom context restrictions that have one.</summary>
        public IEnumerable<SecurityContextRestrictionFilterInfo> GetSafeSecurityContextRestrictionFilters() =>
            from securityContextRestriction in this.GetSafeSecurityContextRestrictions()
            where securityContextRestriction.RawFilter is not null
            select securityContextRestriction.RawFilter;

        /// <summary>Determines whether both rules have the same expand type, credential and restriction.</summary>
        /// <param name="other">The rule to compare with.</param>
        public bool EqualsCustoms(RoleBaseSecurityRule other)
        {
            return this.CustomExpandType == other.CustomExpandType
                   && this.CustomCredential == other.CustomCredential
                   && this.CustomRestriction == other.CustomRestriction;
        }

        /// <summary>Determines whether no customization is set.</summary>
        public bool HasDefaultCustoms()
        {
            return this.CustomExpandType is null && this.CustomCredential is null && this.CustomRestriction is null;
        }

        public static implicit operator RoleBaseSecurityRule(SecurityOperation securityOperation) => securityOperation.ToSecurityRule();

        public static implicit operator RoleBaseSecurityRule(SecurityRole securityRole) => securityRole.ToSecurityRule();

        public static implicit operator RoleBaseSecurityRule(SecurityRole[] securityRoles) => securityRoles.ToSecurityRule();

        public static implicit operator RoleBaseSecurityRule(NonExpandedRolesSecurityRule[] securityRules) => securityRules.ToSecurityRule();
    }

    /// <summary>The customization data of a role-based rule.</summary>
    public interface IRoleBaseSecurityRuleCustomData
    {
        public HierarchicalExpandType? CustomExpandType { get; }

        public SecurityRuleCredential? CustomCredential { get; }

        public SecurityPathRestriction? CustomRestriction { get; }
    }

    /// <summary>Satisfied by any role the user has.</summary>
    public record AnyRoleSecurityRule : RoleBaseSecurityRule;

    /// <summary>Roles are produced by a custom factory at expansion time.</summary>
    /// <param name="RoleFactoryType">The factory type.</param>
    public record RoleFactorySecurityRule(Type RoleFactoryType) : RoleBaseSecurityRule;

    /// <summary>A group of role rules, any of which grants access; before expansion.</summary>
    /// <param name="Children">The rules of the group.</param>
    public record NonExpandedRoleGroupSecurityRule(DeepEqualsCollection<NonExpandedRolesSecurityRule> Children) : RoleBaseSecurityRule
    {
        public NonExpandedRoleGroupSecurityRule(IEnumerable<NonExpandedRolesSecurityRule> children)
            : this(children.ToArray())
        {
        }

        public override string ToString() => this.Children.Count == 1
            ? this.Children.Single().ToString()
            : $"[{this.Children.Join(", ", sr => sr.ToString())}]";
    }

    /// <summary>A group of expanded role rules, any of which grants access; the result of expanding a rule.</summary>
    /// <param name="Children">The rules of the group.</param>
    public record ExpandedRoleGroupSecurityRule(DeepEqualsCollection<ExpandedRolesSecurityRule> Children) : RoleBaseSecurityRule
    {
        public ExpandedRoleGroupSecurityRule(IEnumerable<ExpandedRolesSecurityRule> children)
            : this(children.ToArray())
        {
        }

        /// <summary>The child rules with the customization of the group applied to each of them.</summary>
        public IEnumerable<ExpandedRolesSecurityRule> GetActualChildren() =>
            this.HasDefaultCustoms() ? this.Children : this.Children.Select(c => c.ApplyCustoms(this));

        public override string ToString() => this.Children.Count == 1
            ? this.Children.Single().ToString()
            : $"[{this.Children.Join(", ", sr => sr.ToString())}]";
    }

    /// <summary>Satisfied by the roles that include the operation.</summary>
    /// <param name="SecurityOperation">The operation.</param>
    public record OperationSecurityRule(SecurityOperation SecurityOperation) : RoleBaseSecurityRule
    {
        public override string ToString() => this.SecurityOperation.Name;

        public static implicit operator OperationSecurityRule(SecurityOperation securityOperation) => securityOperation.ToSecurityRule();
    }

    /// <summary>
    /// Roles BEFORE the role tree is expanded upwards.
    /// </summary>
    /// <param name="SecurityRoles">Unexpanded roles</param>
    public record NonExpandedRolesSecurityRule(DeepEqualsCollection<SecurityRole> SecurityRoles) : RoleBaseSecurityRule
    {
        public override string ToString() => this.SecurityRoles.Count == 1
                                                 ? this.SecurityRoles.Single().Name
                                                 : $"[{this.SecurityRoles.Join(", ", sr => sr.Name)}]";

        public static implicit operator NonExpandedRolesSecurityRule(SecurityRole securityRole) => securityRole.ToSecurityRule();

        public static implicit operator NonExpandedRolesSecurityRule(SecurityRole[] securityRoles) => securityRoles.ToSecurityRule();

        public static RoleBaseSecurityRule operator +(NonExpandedRolesSecurityRule rule1, NonExpandedRolesSecurityRule rule2)
        {
            if (!rule1.EqualsCustoms(rule2))
            {
                return new NonExpandedRoleGroupSecurityRule([rule1, rule2]);
            }
            else
            {
                return rule1 with { SecurityRoles = rule1.SecurityRoles.Union(rule2.SecurityRoles).ToArray() };
            }
        }
    }

    /// <summary>
    /// Roles AFTER the role tree is expanded upwards.
    /// </summary>
    /// <param name="SecurityRoles">Expanded roles</param>
    public record ExpandedRolesSecurityRule(DeepEqualsCollection<SecurityRole> SecurityRoles) : RoleBaseSecurityRule
    {
        public ExpandedRolesSecurityRule(IEnumerable<SecurityRole> securityRoles)
            : this(securityRoles.ToArray())
        {
        }

        /// <summary>A rule with no roles.</summary>
        public static ExpandedRolesSecurityRule Empty { get; } = new([]);

        public override string ToString() => this.SecurityRoles.Count == 1
                                                 ? this.SecurityRoles.Single().Name
                                                 : $"[{this.SecurityRoles.Join(", ", sr => sr.Name)}]";

        public static RoleBaseSecurityRule operator +(ExpandedRolesSecurityRule rule1, ExpandedRolesSecurityRule rule2)
        {
            if (!rule1.EqualsCustoms(rule2))
            {
                return new ExpandedRoleGroupSecurityRule([rule1, rule2]);
            }
            else
            {
                return rule1 with { SecurityRoles = rule1.SecurityRoles.Union(rule2.SecurityRoles).ToArray() };
            }
        }
    }

    /// <summary>Satisfied when both rules are satisfied.</summary>
    /// <param name="Left">The first rule.</param>
    /// <param name="Right">The second rule.</param>
    public record AndSecurityRule(DomainSecurityRule Left, DomainSecurityRule Right) : DomainSecurityRule;

    /// <summary>Satisfied when at least one of the rules is satisfied.</summary>
    /// <param name="Left">The first rule.</param>
    /// <param name="Right">The second rule.</param>
    public record OrSecurityRule(DomainSecurityRule Left, DomainSecurityRule Right) : DomainSecurityRule;

    /// <summary>Satisfied when the inner rule is not satisfied.</summary>
    /// <param name="InnerRule">The rule to negate.</param>
    public record NegateSecurityRule(DomainSecurityRule InnerRule) : DomainSecurityRule;
}

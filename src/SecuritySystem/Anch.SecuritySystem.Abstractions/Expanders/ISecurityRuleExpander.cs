namespace Anch.SecuritySystem.Expanders;

/// <summary>Turns any security rule into its final form made of concrete roles. It combines the specialized expanders (modes, operations, role groups, factories, client rules, headers).</summary>
public interface ISecurityRuleExpander : ISecurityModeExpander,
    ISecurityOperationExpander,
    ISecurityRoleGroupExpander,
    IRoleFactorySecurityRuleExpander,
    IClientSecurityRuleExpander,
    ISecurityRuleHeaderExpander
{
    /// <summary>Expands a role-based rule completely.</summary>
    /// <param name="securityRule">The rule to expand.</param>
    DomainSecurityRule.ExpandedRoleGroupSecurityRule FullRoleExpand(DomainSecurityRule.RoleBaseSecurityRule securityRule);

    /// <summary>Expands every part of a rule until only concrete roles, conditions and providers remain.</summary>
    /// <param name="securityRule">The rule to expand.</param>
    DomainSecurityRule FullDomainExpand(DomainSecurityRule securityRule);

    /// <summary>Converts a rule to a domain rule for a type, binding modes to the type.</summary>
    /// <param name="securityRule">The rule to convert.</param>
    /// <typeparam name="TDomainObject">The domain type.</typeparam>
    DomainSecurityRule ToDomain<TDomainObject>(SecurityRule securityRule);
}
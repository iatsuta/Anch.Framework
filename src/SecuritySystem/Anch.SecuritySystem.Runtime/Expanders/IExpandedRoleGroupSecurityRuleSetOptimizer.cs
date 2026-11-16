namespace Anch.SecuritySystem.Expanders;

/// <summary>Simplifies a set of expanded role rules.</summary>
public interface IExpandedRoleGroupSecurityRuleSetOptimizer
{
    DomainSecurityRule.ExpandedRoleGroupSecurityRule Optimize(IEnumerable<DomainSecurityRule.ExpandedRolesSecurityRule> roleRules);

    DomainSecurityRule.ExpandedRoleGroupSecurityRule Optimize(IEnumerable<DomainSecurityRule.ExpandedRoleGroupSecurityRule> roleRules);
}
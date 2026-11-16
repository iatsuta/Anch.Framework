namespace Anch.SecuritySystem.Expanders;

/// <summary>Expands roles upwards through the role hierarchy, so that a role also covers the roles that include it.</summary>
public interface ISecurityRoleGroupExpander
{
    /// <summary>Expands a group of role rules.</summary>
    /// <param name="securityRule">The group.</param>
    DomainSecurityRule.ExpandedRoleGroupSecurityRule Expand(DomainSecurityRule.NonExpandedRoleGroupSecurityRule securityRule);

    /// <summary>Expands a role rule.</summary>
    /// <param name="securityRule">The role rule.</param>
    DomainSecurityRule.ExpandedRoleGroupSecurityRule Expand(DomainSecurityRule.NonExpandedRolesSecurityRule securityRule);
}
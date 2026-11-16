namespace Anch.SecuritySystem.Expanders;

/// <summary>Replaces a role factory rule with the role rule that the factory produces.</summary>
public interface IRoleFactorySecurityRuleExpander
{
    /// <summary>Expands the factory rule.</summary>
    /// <param name="securityRule">The factory rule.</param>
    DomainSecurityRule.RoleBaseSecurityRule Expand(DomainSecurityRule.RoleFactorySecurityRule securityRule);
}

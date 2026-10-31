namespace Anch.SecuritySystem.SecurityRuleInfo;

/// <summary>Finds the domain type modes that a role grants.</summary>
public interface IDomainModeSecurityRuleResolver
{
    /// <summary>Returns the modes the role grants.</summary>
    /// <param name="securityRole">The role.</param>
    IEnumerable<DomainSecurityRule.DomainModeSecurityRule> Resolve(SecurityRole securityRole);
}

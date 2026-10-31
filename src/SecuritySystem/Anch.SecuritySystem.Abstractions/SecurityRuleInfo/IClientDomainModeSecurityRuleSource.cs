namespace Anch.SecuritySystem.SecurityRuleInfo;

/// <summary>Lists the domain type modes that are exposed to clients.</summary>
public interface IClientDomainModeSecurityRuleSource
{
    /// <summary>Returns the exposed rules.</summary>
    IEnumerable<DomainSecurityRule.DomainModeSecurityRule> GetRules();
}

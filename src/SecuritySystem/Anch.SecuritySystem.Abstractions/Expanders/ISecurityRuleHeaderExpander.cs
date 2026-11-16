namespace Anch.SecuritySystem.Expanders;

/// <summary>Replaces a named rule header with the rule registered under that name.</summary>
public interface ISecurityRuleHeaderExpander
{
    /// <summary>Expands the header.</summary>
    /// <param name="securityRuleHeader">The header.</param>
    DomainSecurityRule Expand(DomainSecurityRule.SecurityRuleHeader securityRuleHeader);
}

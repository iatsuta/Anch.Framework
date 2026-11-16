namespace Anch.SecuritySystem.Expanders;

/// <summary>Replaces a named client rule with the rule it stands for.</summary>
public interface IClientSecurityRuleExpander
{
    /// <summary>Expands the client rule.</summary>
    /// <param name="securityRule">The client rule.</param>
    DomainSecurityRule Expand(DomainSecurityRule.ClientSecurityRule securityRule);
}

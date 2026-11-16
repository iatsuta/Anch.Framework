namespace Anch.SecuritySystem.SecurityRuleInfo;

/// <summary>Binds a client rule name to the rule it stands for.</summary>
/// <param name="Rule">The client rule.</param>
/// <param name="Implementation">The rule that is checked.</param>
public record ClientSecurityRuleInfo(DomainSecurityRule.ClientSecurityRule Rule, DomainSecurityRule Implementation)
{
    /// <summary>Creates the binding from a rule name.</summary>
    /// <param name="name">The client rule name.</param>
    /// <param name="implementation">The rule that is checked.</param>
    public ClientSecurityRuleInfo(string name, DomainSecurityRule implementation)
        : this(new DomainSecurityRule.ClientSecurityRule(name), implementation)
    {
    }
}

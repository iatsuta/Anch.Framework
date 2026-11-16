namespace Anch.SecuritySystem.SecurityRuleInfo;

/// <summary>Binds a rule header (a name) to the rule it stands for.</summary>
/// <param name="Header">The header.</param>
/// <param name="Implementation">The rule that replaces the header.</param>
public record SecurityRuleHeaderInfo(DomainSecurityRule.SecurityRuleHeader Header, DomainSecurityRule Implementation);

namespace Anch.SecuritySystem.SecurityRuleInfo;

/// <summary>Binds an access mode of a domain type to the rule that implements it.</summary>
/// <param name="SecurityRule">The mode of a domain type.</param>
/// <param name="Implementation">The rule that is checked.</param>
public record DomainModeSecurityRuleInfo(DomainSecurityRule.DomainModeSecurityRule SecurityRule, DomainSecurityRule Implementation);

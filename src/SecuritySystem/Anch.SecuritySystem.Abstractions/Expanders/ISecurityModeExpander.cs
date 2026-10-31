namespace Anch.SecuritySystem.Expanders;

/// <summary>Replaces an access mode of a domain type (view, edit) with the rule configured for that type.</summary>
public interface ISecurityModeExpander
{
    /// <summary>Expands the mode rule.</summary>
    /// <param name="securityRule">The mode of a domain type.</param>
    /// <returns>The configured rule, or <c>null</c> when none is configured for the type.</returns>
    DomainSecurityRule? TryExpand(DomainSecurityRule.DomainModeSecurityRule securityRule);
}

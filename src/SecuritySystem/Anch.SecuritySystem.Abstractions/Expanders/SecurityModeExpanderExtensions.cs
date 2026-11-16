namespace Anch.SecuritySystem.Expanders;

/// <summary>Extensions for <see cref="ISecurityModeExpander"/>.</summary>
public static class SecurityModeExpanderExtensions
{
    /// <summary>Expands the mode rule; throws when no rule is configured for the type.</summary>
    /// <param name="expander">The expander.</param>
    /// <param name="securityRule">The mode of a domain type.</param>
    /// <exception cref="ArgumentOutOfRangeException">No rule is configured for the mode and type.</exception>
    public static DomainSecurityRule Expand(this ISecurityModeExpander expander, DomainSecurityRule.DomainModeSecurityRule securityRule)
    {
        return expander.TryExpand(securityRule)
               ?? throw new ArgumentOutOfRangeException(
                   nameof(securityRule),
                   $"{nameof(SecurityRule)} with mode '{securityRule}' not found for type '{securityRule.DomainType.Name}'");
    }
}

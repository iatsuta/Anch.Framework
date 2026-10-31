namespace Anch.SecuritySystem;

/// <summary>
/// Describes the access that is required for an operation: which roles, operations, mode or conditions the current user must satisfy.
/// A rule is plain data; it is expanded into roles and checked by <c>ISecuritySystem</c> (for a single check) or turned into an
/// <c>ISecurityProvider&lt;T&gt;</c> by <c>IDomainSecurityService&lt;T&gt;</c> (for domain objects).
/// Roles and operations convert to rules implicitly; use <c>Or</c>, <c>And</c>, <c>Except</c> and <c>Negate</c> to combine rules.
/// </summary>
/// <example>
/// <code>
/// await securitySystem.CheckAccessAsync(SecurityRole.Administrator, ct);
/// var provider = domainSecurityService.GetSecurityProvider(SecurityRule.View);
/// </code>
/// </example>
public abstract record SecurityRule
{
    /// <summary>Overrides whose permissions are checked; <c>null</c> means the current user (with RunAs).</summary>
    public SecurityRuleCredential? CustomCredential { get; init; }


    /// <summary>
    /// Access rule for viewing an object.
    /// </summary>
    public static ModeSecurityRule View { get; } = new(nameof(View));

    /// <summary>
    /// Access rule for editing an object.
    /// </summary>
    public static ModeSecurityRule Edit { get; } = new(nameof(Edit));

    /// <summary>
    /// Access rule that disables security checks.
    /// </summary>
    public static DomainSecurityRule.ProviderSecurityRule Disabled { get; } = new() { Key = nameof(Disabled) };


    public static implicit operator SecurityRule(SecurityOperation securityOperation) => securityOperation.ToSecurityRule();

    public static implicit operator SecurityRule(SecurityRole securityRole) => securityRole.ToSecurityRule();

    public static implicit operator SecurityRule(SecurityRole[] securityRoles) => securityRoles.ToSecurityRule();

    public static implicit operator SecurityRule(DomainSecurityRule.NonExpandedRolesSecurityRule[] securityRules) => securityRules.ToSecurityRule();

    /// <summary>
    /// A rule that names an access mode (<see cref="View"/> or <see cref="Edit"/>) without saying for which type.
    /// The roles that grant the mode are configured per domain type with <c>AddDomainSecurity&lt;T&gt;</c>.
    /// </summary>
    /// <param name="Name">Name of the mode.</param>
    public record ModeSecurityRule(string Name) : SecurityRule
    {
        public override string ToString() => this.Name;

        /// <summary>Binds the mode to a domain type.</summary>
        /// <typeparam name="TDomainObject">The domain type.</typeparam>
        public DomainSecurityRule.DomainModeSecurityRule ToDomain<TDomainObject>() => this.ToDomain(typeof(TDomainObject));

        /// <summary>Binds the mode to a domain type.</summary>
        /// <param name="domainType">The domain type.</param>
        public DomainSecurityRule.DomainModeSecurityRule ToDomain(Type domainType) => new(domainType, this) { CustomCredential = this.CustomCredential };
    }
}
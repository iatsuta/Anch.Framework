namespace Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;

/// <summary>Extensions for <see cref="ISecuritySystem"/>.</summary>
public static class SecuritySystemExtensions
{
    /// <summary>Determines whether the current user is a security administrator.</summary>
    /// <param name="securitySystem">The security system.</param>
    /// <param name="ct">Cancellation token.</param>
    public static Task<bool> IsSecurityAdministratorAsync(this ISecuritySystem securitySystem, CancellationToken ct) =>
        securitySystem.HasAccessAsync(ApplicationSecurityRule.SecurityAdministrator, ct);
}

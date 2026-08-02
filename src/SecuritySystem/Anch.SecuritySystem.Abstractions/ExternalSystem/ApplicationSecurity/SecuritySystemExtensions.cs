namespace Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;

public static class SecuritySystemExtensions
{
    public static Task<bool> IsSecurityAdministratorAsync(this ISecuritySystem securitySystem, CancellationToken ct) =>
        securitySystem.HasAccessAsync(ApplicationSecurityRule.SecurityAdministrator, ct);
}

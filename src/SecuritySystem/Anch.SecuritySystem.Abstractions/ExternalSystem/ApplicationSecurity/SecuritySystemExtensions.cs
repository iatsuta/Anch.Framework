namespace Anch.SecuritySystem.ExternalSystem.ApplicationSecurity;

public static class SecuritySystemExtensions
{
    public static ValueTask<bool> IsSecurityAdministratorAsync(this ISecuritySystem securitySystem, CancellationToken cancellationToken) =>
        securitySystem.HasAccessAsync(ApplicationSecurityRule.SecurityAdministrator, cancellationToken);
}

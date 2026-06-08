namespace Anch.SecuritySystem.Configurator.Interfaces;

public interface IConfiguratorIntegrationEvents
{
    Task PrincipalCreatedAsync(object principal, CancellationToken ct);

    Task PrincipalChangedAsync(object principal, CancellationToken ct);

    Task PrincipalRemovedAsync(object principal, CancellationToken ct);

    Task PermissionCreatedAsync(object permission, CancellationToken ct);

    Task PermissionChangedAsync(object permission, CancellationToken ct);

    Task PermissionRemovedAsync(object permission, CancellationToken ct);
}

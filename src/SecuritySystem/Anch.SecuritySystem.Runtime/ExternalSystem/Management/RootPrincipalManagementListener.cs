using System.Collections.Immutable;

using Anch.Core;

namespace Anch.SecuritySystem.ExternalSystem.Management;

public class RootPrincipalManagementListener(IServiceProxyFactory serviceProxyFactory, IEnumerable<IPrincipalManagementListener> listeners)
    : IRawPrincipalManagementListener
{
    private readonly ImmutableArray<IRawPrincipalManagementListener> safeListeners =

    [
        ..listeners.Select(l =>

            serviceProxyFactory.Create<IRawPrincipalManagementListener>(
                typeof(SafePrincipalManagementListener<,,>).MakeGenericType(l.PrincipalType, l.PermissionType, l.PermissionRestrictionType), l))
    ];

    public async Task PrincipalCreatedAsync(PrincipalData principal, CancellationToken ct)
    {
        foreach (var listener in this.safeListeners)
        {
            await listener.PrincipalCreatedAsync(principal, ct);
        }
    }

    public async Task PrincipalChangedAsync(PrincipalData principal, CancellationToken ct)
    {
        foreach (var listener in this.safeListeners)
        {
            await listener.PrincipalChangedAsync(principal, ct);
        }
    }

    public async Task PrincipalRemovedAsync(PrincipalData principal, CancellationToken ct)
    {
        foreach (var listener in this.safeListeners)
        {
            await listener.PrincipalRemovedAsync(principal, ct);
        }
    }

    public async Task PermissionCreatedAsync(PermissionData permission, CancellationToken ct)
    {
        foreach (var listener in this.safeListeners)
        {
            await listener.PermissionCreatedAsync(permission, ct);
        }
    }

    public async Task PermissionChangedAsync(PermissionData permission, CancellationToken ct)
    {
        foreach (var listener in this.safeListeners)
        {
            await listener.PermissionChangedAsync(permission, ct);
        }
    }

    public async Task PermissionRemovedAsync(PermissionData permission, CancellationToken ct)
    {
        foreach (var listener in this.safeListeners)
        {
            await listener.PermissionRemovedAsync(permission, ct);
        }
    }
}
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

    public async Task PrincipalCreatedAsync(PrincipalData principalData, CancellationToken ct)
    {
        foreach (var listener in this.safeListeners)
        {
            await listener.PrincipalCreatedAsync(principalData, ct);
        }
    }

    public async Task PrincipalChangedAsync(PrincipalData principalData, CancellationToken ct)
    {
        foreach (var listener in this.safeListeners)
        {
            await listener.PrincipalChangedAsync(principalData, ct);
        }
    }

    public async Task PrincipalRemovedAsync(PrincipalData principalData, CancellationToken ct)
    {
        foreach (var listener in this.safeListeners)
        {
            await listener.PrincipalRemovedAsync(principalData, ct);
        }
    }

    public async Task PermissionCreatedAsync(PermissionData permissionData, CancellationToken ct)
    {
        foreach (var listener in this.safeListeners)
        {
            await listener.PermissionCreatedAsync(permissionData, ct);
        }
    }

    public async Task PermissionChangedAsync(PermissionData permissionData, CancellationToken ct)
    {
        foreach (var listener in this.safeListeners)
        {
            await listener.PermissionChangedAsync(permissionData, ct);
        }
    }

    public async Task PermissionRemovedAsync(PermissionData permissionData, CancellationToken ct)
    {
        foreach (var listener in this.safeListeners)
        {
            await listener.PermissionRemovedAsync(permissionData, ct);
        }
    }
}
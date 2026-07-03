using Anch.Core;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.ExternalSystem.Management;

public class RootPrincipalManagementService(
    [FromKeyedServices(IPrincipalManagementService.ElementKey)]
    IEnumerable<IPrincipalManagementService> principalManagementServices,
    IEnumerable<IRawPrincipalManagementListener> listeners)
    : IPrincipalManagementService
{
    private IPrincipalManagementService ActualService => principalManagementServices.Single(
        () => new SecuritySystemException("No writable management service was found"),
        () => new SecuritySystemException("Multiple writable management services were found"));

    public Type PrincipalType => this.ActualService.PrincipalType;

    public async Task<PrincipalData> CreatePrincipalAsync(UserCredential userCredential, IEnumerable<ManagedPermission> managedPermissions, CancellationToken ct)
    {
        var principal = await this.ActualService.CreatePrincipalAsync(userCredential, managedPermissions, ct);

        foreach (var listener in listeners)
        {
            await listener.PrincipalCreatedAsync(principal, ct);
        }

        return principal;
    }

    public async Task<PrincipalData> UpdatePrincipalNameAsync(UserCredential userCredential, string principalName, CancellationToken ct)
    {
        var principal = await this.ActualService.UpdatePrincipalNameAsync(userCredential, principalName, ct);

        foreach (var listener in listeners)
        {
            await listener.PrincipalChangedAsync(principal, ct);
        }

        return principal;
    }

    public async Task<PrincipalData> RemovePrincipalAsync(UserCredential userCredential, bool force, CancellationToken ct)
    {
        var principal = await this.ActualService.RemovePrincipalAsync(userCredential, force, ct);

        foreach (var listener in listeners)
        {
            await listener.PrincipalRemovedAsync(principal, ct);
        }

        return principal;
    }

    public async Task<MergeResult<PermissionData, PermissionData>> UpdatePermissionsAsync(UserCredential userCredential,
        IEnumerable<ManagedPermission> managedPermissions, CancellationToken ct)
    {
        var mergeResult = await this.ActualService.UpdatePermissionsAsync(userCredential, managedPermissions, ct);

        foreach (var listener in listeners)
        {
            foreach (var permission in mergeResult.AddingItems)
            {
                await listener.PermissionCreatedAsync(permission, ct);
            }

            foreach (var (permission, _) in mergeResult.CombineItems)
            {
                await listener.PermissionChangedAsync(permission, ct);
            }

            foreach (var permission in mergeResult.RemovingItems)
            {
                await listener.PermissionRemovedAsync(permission, ct);
            }
        }

        return mergeResult;
    }
}
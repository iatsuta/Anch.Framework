namespace Anch.SecuritySystem.ExternalSystem.Management;

public class SafePrincipalManagementListener<TPrincipal, TPermission, TPermissionRestriction>(
    IPrincipalManagementListener<TPrincipal, TPermission, TPermissionRestriction> listener)
    : IRawPrincipalManagementListener
{
    public async Task PrincipalCreatedAsync(PrincipalData principal, CancellationToken ct)
    {
        if (principal is PrincipalData<TPrincipal, TPermission, TPermissionRestriction> typedPrincipal)
        {
            await listener.PrincipalCreatedAsync(typedPrincipal, ct);
        }
    }

    public async Task PrincipalChangedAsync(PrincipalData principal, CancellationToken ct)
    {
        if (principal is PrincipalData<TPrincipal, TPermission, TPermissionRestriction> typedPrincipal)
        {
            await listener.PrincipalChangedAsync(typedPrincipal, ct);
        }
    }

    public async Task PrincipalRemovedAsync(PrincipalData principal, CancellationToken ct)
    {
        if (principal is PrincipalData<TPrincipal, TPermission, TPermissionRestriction> typedPrincipal)
        {
            await listener.PrincipalRemovedAsync(typedPrincipal, ct);
        }
    }

    public async Task PermissionCreatedAsync(PermissionData permission, CancellationToken ct)
    {
        if (permission is PermissionData<TPermission, TPermissionRestriction> typedPermission)
        {
            await listener.PermissionCreatedAsync(typedPermission, ct);
        }
    }

    public async Task PermissionChangedAsync(PermissionData permission, CancellationToken ct)
    {
        if (permission is PermissionData<TPermission, TPermissionRestriction> typedPermission)
        {
            await listener.PermissionChangedAsync(typedPermission, ct);
        }
    }

    public async Task PermissionRemovedAsync(PermissionData permission, CancellationToken ct)
    {
        if (permission is PermissionData<TPermission, TPermissionRestriction> typedPermission)
        {
            await listener.PermissionRemovedAsync(typedPermission, ct);
        }
    }
}
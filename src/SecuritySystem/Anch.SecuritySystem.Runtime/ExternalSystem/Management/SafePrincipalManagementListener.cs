namespace Anch.SecuritySystem.ExternalSystem.Management;

public class SafePrincipalManagementListener<TPrincipal, TPermission, TPermissionRestriction>(
    IPrincipalManagementListener<TPrincipal, TPermission, TPermissionRestriction> listener)
    : IRawPrincipalManagementListener
{
    public async Task PrincipalCreatedAsync(PrincipalData principalData, CancellationToken ct)
    {
        if (principalData is PrincipalData<TPrincipal, TPermission, TPermissionRestriction> typedPrincipalData)
        {
            await listener.PrincipalCreatedAsync(typedPrincipalData, ct);
        }
    }

    public async Task PrincipalChangedAsync(PrincipalData principalData, CancellationToken ct)
    {
        if (principalData is PrincipalData<TPrincipal, TPermission, TPermissionRestriction> typedPrincipalData)
        {
            await listener.PrincipalChangedAsync(typedPrincipalData, ct);
        }
    }

    public async Task PrincipalRemovedAsync(PrincipalData principalData, CancellationToken ct)
    {
        if (principalData is PrincipalData<TPrincipal, TPermission, TPermissionRestriction> typedPrincipalData)
        {
            await listener.PrincipalRemovedAsync(typedPrincipalData, ct);
        }
    }

    public async Task PermissionCreatedAsync(PermissionData permissionData, CancellationToken ct)
    {
        if (permissionData is PermissionData<TPermission, TPermissionRestriction> typedPermissionData)
        {
            await listener.PermissionCreatedAsync(typedPermissionData, ct);
        }
    }

    public async Task PermissionChangedAsync(PermissionData permissionData, CancellationToken ct)
    {
        if (permissionData is PermissionData<TPermission, TPermissionRestriction> typedPermissionData)
        {
            await listener.PermissionChangedAsync(typedPermissionData, ct);
        }
    }

    public async Task PermissionRemovedAsync(PermissionData permissionData, CancellationToken ct)
    {
        if (permissionData is PermissionData<TPermission, TPermissionRestriction> typedPermissionData)
        {
            await listener.PermissionRemovedAsync(typedPermissionData, ct);
        }
    }
}
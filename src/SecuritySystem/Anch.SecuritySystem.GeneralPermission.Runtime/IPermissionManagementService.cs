using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Creates and changes the permission entities of a principal.</summary>
public interface IPermissionManagementService<in TPrincipal, TPermission, TPermissionRestriction>
{
    Task<ManagedPermission> ToManagedPermissionAsync(TPermission permission, CancellationToken ct);

    Task<PermissionData<TPermission, TPermissionRestriction>> CreatePermissionAsync(
        TPrincipal dbPrincipal,
        ManagedPermission managedPermission,
        CancellationToken ct);

    Task<(PermissionData<TPermission, TPermissionRestriction> PermissonData, bool Updated)> UpdatePermission(
        TPermission dbPermission,
        ManagedPermission managedPermission,
        CancellationToken ct);
}
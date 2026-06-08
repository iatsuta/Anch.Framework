using Anch.Core;

namespace Anch.SecuritySystem.ExternalSystem.Management;

public interface IPrincipalManagementService
{
    Type PrincipalType { get; }

    Task<PrincipalData> CreatePrincipalAsync(UserCredential userCredential, IEnumerable<ManagedPermission> managedPermissions, CancellationToken ct);

    Task<PrincipalData> UpdatePrincipalNameAsync(UserCredential userCredential, string principalName, CancellationToken ct);

    Task<PrincipalData> RemovePrincipalAsync(UserCredential userCredential, bool force, CancellationToken ct);

    Task<MergeResult<PermissionData, PermissionData>> UpdatePermissionsAsync(UserCredential userCredential, IEnumerable<ManagedPermission> managedPermissions,
        CancellationToken ct);
}
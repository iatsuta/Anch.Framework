using Anch.Core;

namespace Anch.SecuritySystem.ExternalSystem.Management;

public class FakePrincipalManagementService : IPrincipalManagementService
{
    public Type PrincipalType => throw new InvalidOperationException();

    public Task<PrincipalData> CreatePrincipalAsync(UserCredential userCredential, IEnumerable<ManagedPermission> managedPermissions, CancellationToken ct)
    {
        throw new InvalidOperationException();
    }

    public Task<PrincipalData> UpdatePrincipalNameAsync(UserCredential userCredential, string principalName, CancellationToken ct)
    {
        throw new InvalidOperationException();
    }

    public Task<PrincipalData> RemovePrincipalAsync(UserCredential userCredential, bool force, CancellationToken ct)
    {
        throw new InvalidOperationException();
    }

    public Task<MergeResult<PermissionData, PermissionData>> UpdatePermissionsAsync(UserCredential userCredential, IEnumerable<ManagedPermission> managedPermissions, CancellationToken ct)
    {
        throw new InvalidOperationException();
    }
}
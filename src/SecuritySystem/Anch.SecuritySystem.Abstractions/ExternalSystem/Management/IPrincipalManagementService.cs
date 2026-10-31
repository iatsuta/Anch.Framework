using Anch.Core;

namespace Anch.SecuritySystem.ExternalSystem.Management;

/// <summary>Creates, changes and removes principals and their permissions in a permission source (for example GeneralPermission).</summary>
public interface IPrincipalManagementService
{
    /// <summary>Key under which the individual services that make up the composite are registered.</summary>
    public const string ElementKey = "Element";


    /// <summary>The principal entity type that the service manages.</summary>
    Type PrincipalType { get; }

    /// <summary>Creates a principal with permissions.</summary>
    /// <param name="userCredential">The user the principal belongs to.</param>
    /// <param name="managedPermissions">The initial permissions.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<PrincipalData> CreatePrincipalAsync(UserCredential userCredential, IEnumerable<ManagedPermission> managedPermissions, CancellationToken ct);

    /// <summary>Renames a principal.</summary>
    /// <param name="userCredential">The principal.</param>
    /// <param name="principalName">The new name.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<PrincipalData> UpdatePrincipalNameAsync(UserCredential userCredential, string principalName, CancellationToken ct);

    /// <summary>Removes a principal.</summary>
    /// <param name="userCredential">The principal.</param>
    /// <param name="force">Remove even if the principal still has permissions.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<PrincipalData> RemovePrincipalAsync(UserCredential userCredential, bool force, CancellationToken ct);

    /// <summary>Makes the permissions of a principal match the given list: adds, updates and removes as needed.</summary>
    /// <param name="userCredential">The principal.</param>
    /// <param name="managedPermissions">The desired permissions.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>What was added, updated and removed.</returns>
    Task<MergeResult<PermissionData, PermissionData>> UpdatePermissionsAsync(UserCredential userCredential, IEnumerable<ManagedPermission> managedPermissions,
        CancellationToken ct);
}
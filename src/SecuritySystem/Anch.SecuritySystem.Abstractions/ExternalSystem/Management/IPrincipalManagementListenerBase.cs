namespace Anch.SecuritySystem.ExternalSystem.Management;

public interface IPrincipalManagementListenerBase<in TPrincipalData, in TPermissionData>
{
    Task PrincipalCreatedAsync(TPrincipalData principalData, CancellationToken ct);

    Task PrincipalChangedAsync(TPrincipalData principalData, CancellationToken ct);

    Task PrincipalRemovedAsync(TPrincipalData principalData, CancellationToken ct);

    Task PermissionCreatedAsync(TPermissionData permissionData, CancellationToken ct);

    Task PermissionChangedAsync(TPermissionData permissionData, CancellationToken ct);

    Task PermissionRemovedAsync(TPermissionData permissionData, CancellationToken ct);
}

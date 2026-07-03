namespace Anch.SecuritySystem.ExternalSystem.Management;

public interface IPrincipalManagementListenerBase<in TPrincipalData, in TPermissionData>
{
    Task PrincipalCreatedAsync(TPrincipalData principal, CancellationToken ct);

    Task PrincipalChangedAsync(TPrincipalData principal, CancellationToken ct);

    Task PrincipalRemovedAsync(TPrincipalData principal, CancellationToken ct);

    Task PermissionCreatedAsync(TPermissionData permission, CancellationToken ct);

    Task PermissionChangedAsync(TPermissionData permission, CancellationToken ct);

    Task PermissionRemovedAsync(TPermissionData permission, CancellationToken ct);
}

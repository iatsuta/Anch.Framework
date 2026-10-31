namespace Anch.SecuritySystem.ExternalSystem.Management;

/// <summary>Receives notifications about changes of principals and permissions, for example to send e-mails or write an audit log.</summary>
/// <typeparam name="TPrincipalData">The type that describes a principal.</typeparam>
/// <typeparam name="TPermissionData">The type that describes a permission.</typeparam>
public interface IPrincipalManagementListenerBase<in TPrincipalData, in TPermissionData>
{
    /// <summary>A principal was created.</summary>
    /// <param name="principalData">The principal.</param>
    /// <param name="ct">Cancellation token.</param>
    Task PrincipalCreatedAsync(TPrincipalData principalData, CancellationToken ct);

    /// <summary>A principal was changed.</summary>
    /// <param name="principalData">The principal.</param>
    /// <param name="ct">Cancellation token.</param>
    Task PrincipalChangedAsync(TPrincipalData principalData, CancellationToken ct);

    /// <summary>A principal was removed.</summary>
    /// <param name="principalData">The principal.</param>
    /// <param name="ct">Cancellation token.</param>
    Task PrincipalRemovedAsync(TPrincipalData principalData, CancellationToken ct);

    /// <summary>A permission was created.</summary>
    /// <param name="permissionData">The permission.</param>
    /// <param name="ct">Cancellation token.</param>
    Task PermissionCreatedAsync(TPermissionData permissionData, CancellationToken ct);

    /// <summary>A permission was changed.</summary>
    /// <param name="permissionData">The permission.</param>
    /// <param name="ct">Cancellation token.</param>
    Task PermissionChangedAsync(TPermissionData permissionData, CancellationToken ct);

    /// <summary>A permission was removed.</summary>
    /// <param name="permissionData">The permission.</param>
    /// <param name="ct">Cancellation token.</param>
    Task PermissionRemovedAsync(TPermissionData permissionData, CancellationToken ct);
}

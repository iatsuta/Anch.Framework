namespace Anch.SecuritySystem.ExternalSystem.Management;

/// <summary>A typed listener for changes of principals and permissions that works with the application's own entities.</summary>
/// <typeparam name="TPrincipal">The principal entity type.</typeparam>
/// <typeparam name="TPermission">The permission entity type.</typeparam>
/// <typeparam name="TPermissionRestriction">The permission restriction entity type.</typeparam>
public interface IPrincipalManagementListener<TPrincipal, TPermission, TPermissionRestriction> :
    IPrincipalManagementListenerBase<
        PrincipalData<TPrincipal, TPermission, TPermissionRestriction>,
        PermissionData<TPermission, TPermissionRestriction>>, IPrincipalManagementListener
{
    Type IPrincipalManagementListener.PrincipalType => typeof(TPrincipal);

    Type IPrincipalManagementListener.PermissionType => typeof(TPermission);

    Type IPrincipalManagementListener.PermissionRestrictionType => typeof(TPermissionRestriction);
}

/// <summary>Untyped description of a listener: the entity types it works with.</summary>
public interface IPrincipalManagementListener
{
    /// <summary>The principal entity type.</summary>
    Type PrincipalType { get; }

    /// <summary>The permission entity type.</summary>
    Type PermissionType { get; }

    /// <summary>The permission restriction entity type.</summary>
    Type PermissionRestrictionType { get; }
}
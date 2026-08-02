namespace Anch.SecuritySystem.ExternalSystem.Management;

public interface IPrincipalManagementListener<TPrincipal, TPermission, TPermissionRestriction> :
    IPrincipalManagementListenerBase<
        PrincipalData<TPrincipal, TPermission, TPermissionRestriction>,
        PermissionData<TPermission, TPermissionRestriction>>, IPrincipalManagementListener
{
    Type IPrincipalManagementListener.PrincipalType => typeof(TPrincipal);

    Type IPrincipalManagementListener.PermissionType => typeof(TPermission);

    Type IPrincipalManagementListener.PermissionRestrictionType => typeof(TPermissionRestriction);
}

public interface IPrincipalManagementListener
{
    Type PrincipalType { get; }

    Type PermissionType { get; }

    Type PermissionRestrictionType { get; }
}
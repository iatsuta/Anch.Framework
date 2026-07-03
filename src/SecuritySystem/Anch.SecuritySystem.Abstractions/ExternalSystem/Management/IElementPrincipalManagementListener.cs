namespace Anch.SecuritySystem.ExternalSystem.Management;

public interface IElementPrincipalManagementListener<TPrincipal, TPermission, TPermissionRestriction> :
    IPrincipalManagementListenerBase<
        PrincipalData<TPrincipal, TPermission, TPermissionRestriction>,
        PermissionData<TPermission, TPermissionRestriction>>;
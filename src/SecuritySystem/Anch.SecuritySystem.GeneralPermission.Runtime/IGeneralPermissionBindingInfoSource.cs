namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Finds the bindings of the GeneralPermission source.</summary>
public interface IGeneralPermissionBindingInfoSource
{
    GeneralPermissionBindingInfo GetForPermission(Type permissionType);

    GeneralPermissionBindingInfo GetForSecurityRole(Type securityRoleType);
}
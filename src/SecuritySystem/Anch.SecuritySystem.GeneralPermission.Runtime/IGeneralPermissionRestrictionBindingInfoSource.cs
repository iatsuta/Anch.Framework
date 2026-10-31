namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Finds the restriction bindings of the GeneralPermission source.</summary>
public interface IGeneralPermissionRestrictionBindingInfoSource
{
    GeneralPermissionRestrictionBindingInfo GetForPermission(Type permissionType);

    GeneralPermissionRestrictionBindingInfo GetForPermissionRestriction(Type permissionRestrictionType);
}
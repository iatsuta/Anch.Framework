namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Finds the security context type that a restriction entity refers to.</summary>
public interface IPermissionRestrictionSecurityContextTypeResolver<in TPermissionRestriction>
{
    Type Resolve(TPermissionRestriction permissionRestriction);
}
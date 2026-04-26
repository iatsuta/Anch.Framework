namespace Anch.SecuritySystem.GeneralPermission;

public interface IPermissionRestrictionSecurityContextTypeResolver<in TPermissionRestriction>
{
    Type Resolve(TPermissionRestriction permissionRestriction);
}
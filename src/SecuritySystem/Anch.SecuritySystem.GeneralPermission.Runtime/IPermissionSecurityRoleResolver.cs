namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Finds the role of a permission entity.</summary>
public interface IPermissionSecurityRoleResolver<in TPermission>
{
    FullSecurityRole Resolve(TPermission permission);
}
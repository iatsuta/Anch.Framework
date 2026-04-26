using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.GeneralPermission.Validation;

public interface IDisplayPermissionService<TPermission, TPermissionRestriction>
{
    string Format(PermissionData<TPermission, TPermissionRestriction> permissionData);
}
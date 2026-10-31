using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.GeneralPermission.Validation;

/// <summary>Formats a permission as text for validation messages.</summary>
public interface IDisplayPermissionService<TPermission, TPermissionRestriction>
{
    string Format(PermissionData<TPermission, TPermissionRestriction> permissionData);
}
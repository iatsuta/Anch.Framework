using Anch.SecuritySystem.ExternalSystem.Management;
using Anch.SecuritySystem.Validation;

namespace Anch.SecuritySystem.GeneralPermission.Validation.Permission;

/// <summary>Validates a permission together with its restrictions.</summary>
public interface IPermissionValidator<TPermission, TPermissionRestriction> : ISecurityValidator<PermissionData<TPermission, TPermissionRestriction>>;
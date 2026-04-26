using Anch.SecuritySystem.ExternalSystem.Management;
using Anch.SecuritySystem.Validation;

namespace Anch.SecuritySystem.GeneralPermission.Validation.Permission;

public interface IPermissionValidator<TPermission, TPermissionRestriction> : ISecurityValidator<PermissionData<TPermission, TPermissionRestriction>>;
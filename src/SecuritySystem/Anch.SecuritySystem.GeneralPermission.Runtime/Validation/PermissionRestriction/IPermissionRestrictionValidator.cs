using Anch.SecuritySystem.Validation;

namespace Anch.SecuritySystem.GeneralPermission.Validation.PermissionRestriction;

public interface IPermissionRestrictionValidator<in TPermissionRestriction> : ISecurityValidator<TPermissionRestriction>;
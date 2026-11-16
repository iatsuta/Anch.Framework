using Anch.SecuritySystem.Validation;

namespace Anch.SecuritySystem.GeneralPermission.Validation.PermissionRestriction;

/// <summary>Validates a permission restriction.</summary>
public interface IPermissionRestrictionValidator<in TPermissionRestriction> : ISecurityValidator<TPermissionRestriction>;
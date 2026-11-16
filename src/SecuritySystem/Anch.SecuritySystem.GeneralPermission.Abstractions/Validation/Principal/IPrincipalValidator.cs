using Anch.SecuritySystem.ExternalSystem.Management;
using Anch.SecuritySystem.Validation;

namespace Anch.SecuritySystem.GeneralPermission.Validation.Principal;

/// <summary>Validates a principal together with its permissions.</summary>
public interface IPrincipalValidator<TPrincipal, TPermission, TPermissionRestriction> : ISecurityValidator<PrincipalData<TPrincipal, TPermission, TPermissionRestriction>>;
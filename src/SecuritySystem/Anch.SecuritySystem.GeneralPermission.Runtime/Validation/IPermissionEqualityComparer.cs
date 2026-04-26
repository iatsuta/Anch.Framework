using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.GeneralPermission.Validation;

public interface IPermissionEqualityComparer<TPermission, TPermissionRestriction> : IEqualityComparer<PermissionData<TPermission, TPermissionRestriction>>;
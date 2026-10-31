using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.GeneralPermission.Validation;

/// <summary>Decides whether two permissions are the same when permissions are updated or validated.</summary>
public interface IPermissionEqualityComparer<TPermission, TPermissionRestriction> : IEqualityComparer<PermissionData<TPermission, TPermissionRestriction>>;
using System.Collections.Immutable;

namespace Anch.SecuritySystem.Services;

/// <summary>Finds how permission entities are bound to principals and roles.</summary>
public interface IPermissionBindingInfoSource
{
    PermissionBindingInfo GetForPermission(Type permissionType);

    ImmutableArray<PermissionBindingInfo> GetForPrincipal(Type principalType);
}
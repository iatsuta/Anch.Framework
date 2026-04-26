using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Anch.SecuritySystem.Services;

public class PermissionBindingInfoSource : IPermissionBindingInfoSource
{
    private readonly FrozenDictionary<Type, PermissionBindingInfo> permissionDict;

    private readonly FrozenDictionary<Type, ImmutableArray<PermissionBindingInfo>> principalDict;

    public PermissionBindingInfoSource(IEnumerable<PermissionBindingInfo> bindingInfoList)
    {
        var cache = bindingInfoList.ToList();

        this.principalDict = cache.GroupBy(v => v.PrincipalType).ToFrozenDictionary(g => g.Key, g => g.ToImmutableArray());
        this.permissionDict = cache.ToFrozenDictionary(v => v.PermissionType);
    }

    public PermissionBindingInfo GetForPermission(Type permissionType) => this.permissionDict[permissionType];

    public ImmutableArray<PermissionBindingInfo> GetForPrincipal(Type principalType) => this.principalDict[principalType];
}
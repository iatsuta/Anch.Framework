using System.Collections.Frozen;

namespace Anch.SecuritySystem.GeneralPermission;

public class GeneralPermissionBindingInfoSource : IGeneralPermissionBindingInfoSource
{
    private readonly FrozenDictionary<Type, GeneralPermissionBindingInfo> permissionDict;

    private readonly FrozenDictionary<Type, GeneralPermissionBindingInfo> securityRoleDict;

    public GeneralPermissionBindingInfoSource(IEnumerable<GeneralPermissionBindingInfo> bindingInfoList)
    {
        var cache = bindingInfoList.ToList();

        this.permissionDict = cache.ToFrozenDictionary(v => v.PermissionType);
        this.securityRoleDict = cache.ToFrozenDictionary(v => v.SecurityRoleType);
    }

    public GeneralPermissionBindingInfo GetForPermission(Type permissionType) => this.permissionDict[permissionType];

    public GeneralPermissionBindingInfo GetForSecurityRole(Type securityRoleType) => this.securityRoleDict[securityRoleType];
}
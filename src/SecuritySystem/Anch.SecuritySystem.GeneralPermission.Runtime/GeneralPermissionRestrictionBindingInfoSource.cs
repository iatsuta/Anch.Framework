using System.Collections.Frozen;

namespace Anch.SecuritySystem.GeneralPermission;

public class GeneralPermissionRestrictionBindingInfoSource : IGeneralPermissionRestrictionBindingInfoSource
{
    private readonly FrozenDictionary<Type, GeneralPermissionRestrictionBindingInfo> permissionDict;

    private readonly FrozenDictionary<Type, GeneralPermissionRestrictionBindingInfo> permissionRestrictionDict;

    public GeneralPermissionRestrictionBindingInfoSource(IEnumerable<GeneralPermissionRestrictionBindingInfo> bindingInfoList)
    {
        var cache = bindingInfoList.ToList();

        this.permissionDict = cache.ToFrozenDictionary(v => v.PermissionType);
        this.permissionRestrictionDict = cache.ToFrozenDictionary(v => v.PermissionRestrictionType);
    }


    public GeneralPermissionRestrictionBindingInfo GetForPermission(Type permissionType) => this.permissionDict[permissionType];

    public GeneralPermissionRestrictionBindingInfo GetForPermissionRestriction(Type permissionRestrictionType) =>
        this.permissionRestrictionDict[permissionRestrictionType];
}
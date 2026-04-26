namespace Anch.SecuritySystem.GeneralPermission;

public class PermissionRestrictionRawConverter<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>(

    GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent> restrictionBindingInfo,
    IPermissionRestrictionSecurityContextTypeResolver<TPermissionRestriction> permissionRestrictionSecurityContextTypeResolver)
    : IPermissionRestrictionRawConverter<TPermissionRestriction>
{
    public Dictionary<Type, Array> Convert(IEnumerable<TPermissionRestriction> permissionRestrictions)
    {
        return permissionRestrictions.GroupBy(permissionRestrictionSecurityContextTypeResolver.Resolve, restrictionBindingInfo.SecurityContextObjectId.Getter)
            .ToDictionary(g => g.Key, Array (g) => g.ToArray());
    }
}
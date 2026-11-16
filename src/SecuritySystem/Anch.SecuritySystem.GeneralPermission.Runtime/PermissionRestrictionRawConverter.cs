namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Converts restriction entities into pairs of a security context type and an object identifier.</summary>
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
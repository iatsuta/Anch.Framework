namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Converts restriction entities into pairs of a security context type and an object identifier.</summary>
public interface IPermissionRestrictionRawConverter<in TPermissionRestriction>
{
    Dictionary<Type, Array> Convert(IEnumerable<TPermissionRestriction> permissionRestrictions);
}
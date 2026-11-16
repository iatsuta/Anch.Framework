namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Converts a permission and its restrictions into the raw form.</summary>
public interface IRawPermissionConverter<in TPermissionRestriction>
{
    Dictionary<Type, Array> ConvertPermission(DomainSecurityRule.RoleBaseSecurityRule securityRule, IEnumerable<TPermissionRestriction> restrictions, IEnumerable<Type> securityContextTypes);
}
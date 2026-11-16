namespace Anch.SecuritySystem.Services;

/// <summary>Provides the permissions of the current user that satisfy a rule.</summary>
/// <typeparam name="TPermission">The permission entity type.</typeparam>
public interface IAvailablePermissionSource<out TPermission>
{
    /// <summary>Returns the matching permissions as a query.</summary>
    /// <param name="securityRule">The rule the permissions must satisfy.</param>
    IQueryable<TPermission> GetQueryable(DomainSecurityRule.RoleBaseSecurityRule securityRule);
}
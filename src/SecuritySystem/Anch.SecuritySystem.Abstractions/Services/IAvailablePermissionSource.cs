namespace Anch.SecuritySystem.Services;

public interface IAvailablePermissionSource<out TPermission>
{
    IQueryable<TPermission> GetQueryable(DomainSecurityRule.RoleBaseSecurityRule securityRule);
}
using Anch.GenericRepository;

namespace Anch.SecuritySystem.Services;

/// <summary>Provides the permissions of the current user that satisfy a rule.</summary>
public class AvailablePermissionSource<TPermission>(
    IQueryableSource queryableSource,
    IAvailablePermissionFilterFactory<TPermission> availablePermissionFilterFactory) : IAvailablePermissionSource<TPermission>
    where TPermission : class
{
    public IQueryable<TPermission> GetQueryable(DomainSecurityRule.RoleBaseSecurityRule securityRule)
    {
        return queryableSource.GetQueryable<TPermission>().Where(availablePermissionFilterFactory.CreateFilter(securityRule));
    }
}
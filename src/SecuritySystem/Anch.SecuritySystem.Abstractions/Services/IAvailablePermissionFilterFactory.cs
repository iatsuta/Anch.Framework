using System.Linq.Expressions;

namespace Anch.SecuritySystem.Services;

public interface IAvailablePermissionFilterFactory<TPermission>
{
    Expression<Func<TPermission, bool>> CreateFilter(DomainSecurityRule.RoleBaseSecurityRule securityRule);
}
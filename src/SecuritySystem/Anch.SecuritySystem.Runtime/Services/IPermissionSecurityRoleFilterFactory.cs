using System.Linq.Expressions;

namespace Anch.SecuritySystem.Services;

public interface IPermissionSecurityRoleFilterFactory<TPermission>
{
    Expression<Func<TPermission, bool>> CreateFilter(DomainSecurityRule.RoleBaseSecurityRule securityRule);
}
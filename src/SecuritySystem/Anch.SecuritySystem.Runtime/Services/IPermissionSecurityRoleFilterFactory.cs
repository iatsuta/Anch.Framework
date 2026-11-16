using System.Linq.Expressions;

namespace Anch.SecuritySystem.Services;

/// <summary>Builds the filter that selects the permissions that have given roles.</summary>
public interface IPermissionSecurityRoleFilterFactory<TPermission>
{
    Expression<Func<TPermission, bool>> CreateFilter(DomainSecurityRule.RoleBaseSecurityRule securityRule);
}
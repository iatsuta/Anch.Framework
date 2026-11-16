using System.Collections.Concurrent;
using System.Linq.Expressions;

using Anch.Core;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Builds the filter that selects the permissions that have the roles of a rule.</summary>
public class GeneralPermissionSecurityRoleFilterFactory<TPermission>(
    IPermissionSecurityRoleByIdentsFilterFactory<TPermission> permissionSecurityRoleByIdentsFilterFactory,
    ISecurityRoleIdentsResolver securityRolesIdentsResolver) : IPermissionSecurityRoleFilterFactory<TPermission>
{
    private readonly ConcurrentDictionary<DomainSecurityRule.RoleBaseSecurityRule, Expression<Func<TPermission, bool>>> cache = [];

    public Expression<Func<TPermission, bool>> CreateFilter(DomainSecurityRule.RoleBaseSecurityRule securityRule) =>
        this.cache.GetOrAdd(securityRule.WithDefaultCustoms(), _ => this.GetFilterElements(securityRule).BuildAnd());

    private IEnumerable<Expression<Func<TPermission, bool>>> GetFilterElements(DomainSecurityRule.RoleBaseSecurityRule securityRule)
    {
        foreach (var (securityRoleIdentType, securityRoleIdents) in securityRolesIdentsResolver.Resolve(securityRule))
        {
            yield return permissionSecurityRoleByIdentsFilterFactory.CreateFilter(securityRoleIdentType, securityRoleIdents);
        }
    }
}
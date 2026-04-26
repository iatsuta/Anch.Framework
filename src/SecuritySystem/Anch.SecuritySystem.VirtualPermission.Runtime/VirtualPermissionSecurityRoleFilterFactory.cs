using System.Collections.Concurrent;
using System.Linq.Expressions;

using Anch.Core;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.VirtualPermission;

public class VirtualPermissionSecurityRoleFilterFactory<TPermission>(
    IServiceProvider serviceProvider,
    VirtualPermissionBindingInfo<TPermission> virtualPermissionBindingInfo,
    ISecurityRoleResolver securityRoleResolver) : IPermissionSecurityRoleFilterFactory<TPermission>
{
    private readonly ConcurrentDictionary<DomainSecurityRule.RoleBaseSecurityRule, Expression<Func<TPermission, bool>>> cache = [];

    public Expression<Func<TPermission, bool>> CreateFilter(DomainSecurityRule.RoleBaseSecurityRule securityRule) =>
        this.cache.GetOrAdd(securityRule.WithDefaultCustoms(), _ => this.GetFilterElements(securityRule).BuildAnd());

    private IEnumerable<Expression<Func<TPermission, bool>>> GetFilterElements(DomainSecurityRule.RoleBaseSecurityRule securityRule)
    {
        var securityRoles = securityRoleResolver.Resolve(securityRule, true).Items;

        foreach (var virtualPermissionBindingInfoItem in virtualPermissionBindingInfo.Items)
        {
            if (securityRoles.Contains(virtualPermissionBindingInfoItem.SecurityRole))
            {
                yield return virtualPermissionBindingInfoItem.Filter(serviceProvider);
            }
        }
    }
}
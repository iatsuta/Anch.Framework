using System.Linq.Expressions;

using CommonFramework;

using SecuritySystem.Services;

namespace SecuritySystem.VirtualPermission;

public class VirtualPermissionTypedRestrictionBindingInfo<TPermission>(VirtualPermissionBindingInfo<TPermission> bindingInfo)
    : IPermissionTypedRestrictionBindingInfo<TPermission>
{
    public Expression<Func<TPermission, IQueryable<TSecurityContext>>> GetRestrictionsPath<TSecurityContext>()
        where TSecurityContext : class, ISecurityContext =>
        bindingInfo.GetRestrictionsExpr<TSecurityContext, TSecurityContext>(null, v => v).Select(c => c.AsQueryable());
}
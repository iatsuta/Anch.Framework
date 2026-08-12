using System.Linq.Expressions;

using Anch.Core;
using Anch.Core.ExpressionEvaluate;
using Anch.GenericRepository;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.VirtualPermission;

public class VirtualPermissionTypedRestrictionBindingInfo<TPermission>(
    VirtualPermissionBindingInfo<TPermission> bindingInfo,
    IQueryableSource queryableSource)
    : IPermissionTypedRestrictionBindingInfo<TPermission>
{
    public Expression<Func<TPermission, IQueryable<TSecurityContext>>> GetRestrictionsPath<TSecurityContext>()
        where TSecurityContext : class, ISecurityContext
    {
        return ExpressionEvaluateHelper.InlineEvaluate<Func<TPermission, IQueryable<TSecurityContext>>>(ee =>
        {
            var queryable = queryableSource.GetQueryable<TSecurityContext>();

            var filter = this.GetRestrictionsFilters<TSecurityContext>(ee).BuildOr();

            return permission => queryable.Where(ctx =>
                ee.Evaluate(filter, permission, ctx));
        });
    }

    private IEnumerable<Expression<Func<TPermission, TSecurityContext, bool>>> GetRestrictionsFilters<TSecurityContext>(IExpressionEvaluator ee)
        where TSecurityContext : class, ISecurityContext
    {
        foreach (var restriction in bindingInfo.Restrictions)
        {
            if (restriction is Expression<Func<TPermission, TSecurityContext>> singlePath)
            {
                yield return (permission, ctx) => ee.Evaluate(singlePath, permission) == ctx;
            }
            else if (restriction is Expression<Func<TPermission, IEnumerable<TSecurityContext>>> manyPath)
            {
                yield return (permission, ctx) => ee.Evaluate(manyPath, permission).Contains(ctx);
            }
        }
    }
}
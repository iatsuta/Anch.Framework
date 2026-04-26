using System.Linq.Expressions;

using Anch.Core;
using Anch.Core.ExpressionEvaluate;
using Anch.IdentitySource;
using Anch.SecuritySystem.ExternalSystem;

namespace Anch.SecuritySystem.VirtualPermission;

public class VirtualPermissionRestrictionSource<TPermission, TSecurityContext, TSecurityContextIdent>(
    IServiceProvider serviceProvider,
    IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo,
    VirtualPermissionBindingInfo<TPermission> virtualBindingInfo,
    Tuple<SecurityContextRestrictionFilterInfo<TSecurityContext>?> restrictionFilterInfoWrapper)
    : IPermissionRestrictionSource<TPermission, TSecurityContextIdent>

    where TSecurityContext : class, ISecurityContext
    where TSecurityContextIdent : notnull
{
    private readonly SecurityContextRestrictionFilterInfo<TSecurityContext>? restrictionFilterInfo = restrictionFilterInfoWrapper.Item1;

    public Expression<Func<TPermission, IEnumerable<TSecurityContextIdent>>> GetIdentsExpr() =>
        virtualBindingInfo.GetRestrictionsExpr(this.restrictionFilterInfo?.GetPureFilter(serviceProvider), identityInfo.Id.Path);

    public Expression<Func<TPermission, bool>> GetUnrestrictedFilter() => this.GetManyUnrestrictedFilter().BuildAnd();

    public Expression<Func<TPermission, bool>> GetContainsIdentsExpr(IEnumerable<TSecurityContextIdent> idents) =>
        this.GetManyContainsIdentsExpr(idents).BuildOr();

    private IEnumerable<Expression<Func<TPermission, bool>>> GetManyUnrestrictedFilter()
    {
        foreach (var restrictionPath in virtualBindingInfo.Restrictions)
        {
            if (restrictionPath is Expression<Func<TPermission, TSecurityContext?>> singlePath)
            {
                yield return singlePath.Select(securityContext => securityContext == null);
            }
            else if (restrictionPath is Expression<Func<TPermission, IEnumerable<TSecurityContext>>> manyPath)
            {
                yield return manyPath.Select(securityContexts => !securityContexts.Any());
            }
        }
    }

    private IEnumerable<Expression<Func<TPermission, bool>>> GetManyContainsIdentsExpr(IEnumerable<TSecurityContextIdent> idents)
    {
        var filterExpr = identityInfo.CreateFilter(idents);

        foreach (var restrictionPath in virtualBindingInfo.Restrictions)
        {
            if (restrictionPath is Expression<Func<TPermission, TSecurityContext>> singlePath)
            {
                if (this.restrictionFilterInfo == null)
                {
                    yield return singlePath.Select(filterExpr);
                }
                else
                {
                    var securityContextFilter = this.restrictionFilterInfo.GetPureFilter(serviceProvider)
                        .BuildAnd(filterExpr);

                    yield return singlePath.Select(securityContextFilter);
                }
            }
            else if (restrictionPath is Expression<Func<TPermission, IEnumerable<TSecurityContext>>> manyPath)
            {
                yield return ExpressionEvaluateHelper.InlineEvaluate(ee =>
                {
                    if (this.restrictionFilterInfo == null)
                    {
                        return manyPath.Select(securityContexts => securityContexts.Any(securityContext => ee.Evaluate(filterExpr, securityContext)));
                    }
                    else
                    {
                        var securityContextFilter = this.restrictionFilterInfo.GetPureFilter(serviceProvider).ToEnumerableAny()
                            .BuildAnd(securityContexts => securityContexts.Any(securityContext => ee.Evaluate(filterExpr, securityContext)));

                        return manyPath.Select(securityContextFilter);
                    }
                });
            }
        }
    }
}
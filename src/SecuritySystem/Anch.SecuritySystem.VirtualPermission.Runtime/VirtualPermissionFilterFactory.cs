using System.Linq.Expressions;
using Anch.Core;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.VirtualPermission;

public class VirtualPermissionFilterFactory<TPermission>(
    IServiceProvider serviceProvider,
    VirtualPermissionBindingInfo<TPermission> virtualPermissionBindingInfo) : IPermissionFilterFactory<TPermission>
{
    public Expression<Func<TPermission, bool>> CreateFilter(SecurityContextRestriction securityContextRestriction) =>

        new Func<SecurityContextRestriction<ISecurityContext>, Expression<Func<TPermission, bool>>>(this.CreateFilter)
            .CreateGenericMethod(securityContextRestriction.SecurityContextType)
            .Invoke<Expression<Func<TPermission, bool>>>(this, securityContextRestriction);

    public Expression<Func<TPermission, bool>> CreateFilter<TSecurityContext>(
        SecurityContextRestriction<TSecurityContext> securityContextRestriction)
        where TSecurityContext : class, ISecurityContext
    {
        if (securityContextRestriction.Filter != null)
        {
            var restrictionFilter = this.CreateFilter(securityContextRestriction.Filter.GetPureFilter(serviceProvider));

            if (securityContextRestriction.Required)
            {
                return restrictionFilter;
            }
            else
            {
                return this.CreateAnyFilter<TSecurityContext>().Not().BuildOr(restrictionFilter);
            }
        }
        else
        {
            if (securityContextRestriction.Required)
            {
                return this.CreateAnyFilter<TSecurityContext>();
            }
            else
            {
                return _ => true;
            }
        }
    }

    private Expression<Func<TPermission, bool>> CreateAnyFilter<TSecurityContext>()

        where TSecurityContext : class, ISecurityContext => this.CreateFilter<TSecurityContext>(_ => true);

    private Expression<Func<TPermission, bool>> CreateFilter<TSecurityContext>(Expression<Func<TSecurityContext, bool>> securityContextFilter)
        where TSecurityContext : class, ISecurityContext =>

        virtualPermissionBindingInfo.GetRestrictionsExpr(securityContextFilter, v => v).Select(items => items.Any());
}
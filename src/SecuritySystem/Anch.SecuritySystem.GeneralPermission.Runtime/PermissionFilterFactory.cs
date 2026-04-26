using System.Linq.Expressions;

using Anch.Core;
using Anch.GenericRepository;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.GeneralPermission;

public class GeneralPermissionFilterFactory<TPermission, TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>(
    GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent, TPermission> restrictionBindingInfo,
    IQueryableSource queryableSource,
    IPermissionRestrictionFilterFactory<TPermissionRestriction> permissionRestrictionFilterFactory,
    IPermissionRestrictionTypeFilterFactory<TPermissionRestriction> permissionRestrictionTypeFilterFactory) : IPermissionFilterFactory<TPermission>
    where TPermissionRestriction : class
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
            var restrictionFilter = this.CreateFilter(permissionRestrictionFilterFactory.CreateFilter(securityContextRestriction.Filter));

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
        where TSecurityContext : class, ISecurityContext =>
        this.CreateFilter(permissionRestrictionTypeFilterFactory.CreateFilter<TSecurityContext>());

    private Expression<Func<TPermission, bool>> CreateFilter(Expression<Func<TPermissionRestriction, bool>> permissionRestrictionFilter)
    {
        var permissionRestrictionQueryable = queryableSource
            .GetQueryable<TPermissionRestriction>()
            .Where(permissionRestrictionFilter)
            .Select(restrictionBindingInfo.Permission.Path);

        return permission => permissionRestrictionQueryable.Contains(permission);
    }
}
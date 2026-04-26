using System.Linq.Expressions;
using Anch.Core;
using Anch.Core.ExpressionEvaluate;
using Anch.GenericRepository;
using Anch.IdentitySource;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.GeneralPermission;

public class GeneralPermissionTypedRestrictionBindingInfo<TPermission, TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>(
    IQueryableSource queryableSource,
    GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent, TPermission> restrictionBindingInfo,
    IPermissionRestrictionTypeFilterFactory<TPermissionRestriction> permissionRestrictionTypeFilterFactory,
    IIdentityInfoSource identityInfoSource) : IPermissionTypedRestrictionBindingInfo<TPermission>
    where TPermission : class
    where TPermissionRestriction : class
    where TSecurityContextObjectIdent : notnull
{
    public Expression<Func<TPermission, IQueryable<TSecurityContext>>> GetRestrictionsPath<TSecurityContext>()
        where TSecurityContext : class, ISecurityContext
    {
        var securityContextQ = queryableSource.GetQueryable<TSecurityContext>();

        var restrictionQueryable = queryableSource
            .GetQueryable<TPermissionRestriction>()
            .Where(permissionRestrictionTypeFilterFactory.CreateFilter<TSecurityContext>());

        var securityContextIdentityInfo = identityInfoSource.GetIdentityInfo<TSecurityContext, TSecurityContextObjectIdent>();

        var eqIdentExpr = ExpressionHelper.GetEquality<TSecurityContextObjectIdent>();

        return ExpressionEvaluateHelper.InlineEvaluate<Func<TPermission, IQueryable<TSecurityContext>>>(ee =>
        {
            return permission => securityContextQ.Where(sc =>

                restrictionQueryable
                    .Where(restriction => ee.Evaluate(restrictionBindingInfo.Permission.Path, restriction) == permission)
                    .Select(restrictionBindingInfo.SecurityContextObjectId.Path)
                    .Any(securityContextId =>
                        ee.Evaluate(eqIdentExpr, securityContextId, ee.Evaluate(securityContextIdentityInfo.Id.Path, sc)))
            );
        });
    }

    public Expression<Func<TPermission, bool>> GetUnrestrictedFilter<TSecurityContext>()
        where TSecurityContext : class, ISecurityContext
    {
        var restrictionQueryable = queryableSource
            .GetQueryable<TPermissionRestriction>()
            .Where(permissionRestrictionTypeFilterFactory.CreateFilter<TSecurityContext>());

        return ExpressionEvaluateHelper.InlineEvaluate<Func<TPermission, bool>>(ee =>
        {
            return permission => restrictionQueryable
                .All(restriction => ee.Evaluate(restrictionBindingInfo.Permission.Path, restriction) != permission);
        });
    }
}

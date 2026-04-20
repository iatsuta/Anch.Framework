using System.Collections.Concurrent;
using System.Linq.Expressions;

using CommonFramework;
using CommonFramework.IdentitySource;

using SecuritySystem.Services;

namespace SecuritySystem.GeneralPermission;

public class PermissionRestrictionTypeFilterFactory<TPermissionRestriction, TSecurityContextType, TSecurityContextTypeIdent>(
    GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType> restrictionBindingInfo,
    ISecurityContextInfoSource securityContextInfoSource,
    ISecurityIdentityConverter<TSecurityContextTypeIdent> securityContextTypeIdentConverter,
    IIdentityInfo<TSecurityContextType, TSecurityContextTypeIdent> securityContextTypeIdentityInfo)
    : IPermissionRestrictionTypeFilterFactory<TPermissionRestriction>

    where TSecurityContextTypeIdent : notnull
{
    private readonly ConcurrentDictionary<Type, LambdaExpression> cache = [];

    public Expression<Func<TPermissionRestriction, bool>> CreateFilter<TSecurityContext>()
        where TSecurityContext : class, ISecurityContext
    {
        return this.cache.GetOrAddAs(typeof(TSecurityContext), _ =>
        {
            var securityContextTypeId = securityContextTypeIdentConverter.Convert(securityContextInfoSource.GetSecurityContextInfo<TSecurityContext>().Identity)
                .Id;

            var isSecurityContextTypeExpr = ExpressionHelper.GetEqualityWithExpr(securityContextTypeId);

            return restrictionBindingInfo.SecurityContextType.Path.Select(securityContextTypeIdentityInfo.Id.Path)
                .Select(isSecurityContextTypeExpr);
        });
    }
}
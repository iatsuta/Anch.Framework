using System.Linq.Expressions;

using Anch.Core;
using Anch.IdentitySource;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.GeneralPermission;

public class PermissionRestrictionFilterFactory<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>(
    GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent> restrictionBindingInfo,
    IIdentityInfoSource identityInfoSource,
    ISecurityContextSource securityContextSource,
    ISecurityIdentityConverter<TSecurityContextObjectIdent> securityContextObjectIdentConverter)
    : IPermissionRestrictionFilterFactory<TPermissionRestriction>

    where TSecurityContextObjectIdent : notnull
{
    public Expression<Func<TPermissionRestriction, bool>> CreateFilter<TSecurityContext>(SecurityContextRestrictionFilterInfo<TSecurityContext> restrictionFilterInfo)
        where TSecurityContext : class, ISecurityContext
    {
        var identityInfo = identityInfoSource.GetIdentityInfo<TSecurityContext>();

        return new Func<SecurityContextRestrictionFilterInfo<ISecurityContext>, IIdentityInfo<ISecurityContext, Ignore>,
                Expression<Func<TPermissionRestriction, bool>>>(this.CreateRestrictionFilter)
            .CreateGenericMethod(typeof(TSecurityContext), identityInfo.IdentityType)
            .Invoke<Expression<Func<TPermissionRestriction, bool>>>(this, restrictionFilterInfo, identityInfo);
    }

    private Expression<Func<TPermissionRestriction, bool>> CreateRestrictionFilter<TSecurityContext, TSecurityContextIdent>(
        SecurityContextRestrictionFilterInfo<TSecurityContext> restrictionFilterInfo,
        IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo)
        where TSecurityContext : class, ISecurityContext
        where TSecurityContextIdent : notnull
    {
        var convertExpr = securityContextObjectIdentConverter.GetConvertExpression<TSecurityContextIdent>();

        var securityContextQueryable = securityContextSource.GetQueryable(restrictionFilterInfo).Select(identityInfo.Id.Path).Select(convertExpr);

        return restrictionBindingInfo.SecurityContextObjectId.Path.Select(i => securityContextQueryable.Contains(i));
    }
}
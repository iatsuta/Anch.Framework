using System.Linq.Expressions;
using Anch.Core;
using Anch.VisualIdentitySource;

namespace Anch.SecuritySystem.Services;

public class AvailablePermissionFilterFactory<TPermission>(
    IServiceProxyFactory serviceProxyFactory,
    PermissionBindingInfo<TPermission> bindingInfo) : IAvailablePermissionFilterFactory<TPermission>
{
    private readonly Lazy<IAvailablePermissionFilterFactory<TPermission>> lazyInnerService = new(() =>
    {
        var innerServiceType = typeof(AvailablePermissionFilterFactory<,>).MakeGenericType(bindingInfo.PrincipalType, bindingInfo.PermissionType);

        return serviceProxyFactory.Create<IAvailablePermissionFilterFactory<TPermission>>(innerServiceType, bindingInfo);
    });

    public Expression<Func<TPermission, bool>> CreateFilter(DomainSecurityRule.RoleBaseSecurityRule securityRule) =>
        this.lazyInnerService.Value.CreateFilter(securityRule);
}

public class AvailablePermissionFilterFactory<TPrincipal, TPermission>(
    PermissionBindingInfo<TPermission, TPrincipal> bindingInfo,
    TimeProvider timeProvider,
    IUserNameResolver<TPrincipal> userNameResolver,
    IPermissionSecurityRoleFilterFactory<TPermission> permissionSecurityRoleFilterFactory,
    IPermissionFilterFactory<TPermission> permissionFilterFactory,
    SecurityRuleCredential defaultSecurityRuleCredential,
    IVisualIdentityInfo<TPrincipal> principalVisualIdentityInfo,
    IDefaultCancellationTokenSource? defaultCancellationTokenSource = null) : IAvailablePermissionFilterFactory<TPermission>
{
    public Expression<Func<TPermission, bool>> CreateFilter(DomainSecurityRule.RoleBaseSecurityRule securityRule) =>
        this.GetFilterElements(securityRule).BuildAnd();

    private IEnumerable<Expression<Func<TPermission, bool>>> GetFilterElements(DomainSecurityRule.RoleBaseSecurityRule securityRule)
    {
        if (bindingInfo.PermissionStartDate != null)
        {
            yield return bindingInfo.GetPeriodFilter(timeProvider.GetUtcNow().Date);
        }

        var principalName =
            defaultCancellationTokenSource.RunSync(ct => userNameResolver.GetUserNameAsync(securityRule.CustomCredential ?? defaultSecurityRuleCredential, ct));

        if (principalName != null)
        {
            yield return bindingInfo.Principal.Path.Select(principalVisualIdentityInfo.Name.Path).Select(name => name == principalName);
        }

        yield return permissionSecurityRoleFilterFactory.CreateFilter(securityRule);

        foreach (var securityContextRestriction in securityRule.GetSafeSecurityContextRestrictions())
        {
            yield return permissionFilterFactory.CreateFilter(securityContextRestriction);
        }
    }
}
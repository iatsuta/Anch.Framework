using System.Collections.Immutable;
using System.Linq.Expressions;

using Anch.Core;
using Anch.Core.ExpressionEvaluate;
using Anch.GenericQueryable;
using Anch.GenericRepository;
using Anch.IdentitySource;
using Anch.SecuritySystem.ExternalSystem;
using Anch.SecuritySystem.Services;
using Anch.VisualIdentitySource;

namespace Anch.SecuritySystem.VirtualPermission;

public class VirtualPermissionSource<TPrincipal, TPermission>(
    IServiceProvider serviceProvider,
    IExpressionEvaluatorStorage expressionEvaluatorStorage,
    IIdentityInfoSource identityInfoSource,
    IUserNameResolver<TPrincipal> userNameResolver,
    IQueryableSource queryableSource,
    TimeProvider timeProvider,
    SecurityRuleCredential defaultSecurityRuleCredential,
    PermissionBindingInfo<TPermission, TPrincipal> bindingInfo,
    VirtualPermissionBindingInfo<TPermission> virtualBindingInfo,
    VirtualPermissionSecurityRoleItemBindingInfo<TPermission> itemBindingInfo,
    IVisualIdentityInfo<TPrincipal> principalVisualIdentityInfo,
    DomainSecurityRule.RoleBaseSecurityRule securityRule,
    IDefaultCancellationTokenSource? defaultCancellationTokenSource = null) : IPermissionSource<TPermission>
    where TPermission : class
{
    private readonly IExpressionEvaluator expressionEvaluator = expressionEvaluatorStorage.GetForType(typeof(VirtualPermissionSource<TPrincipal, TPermission>));

    private readonly Expression<Func<TPermission, string>> fullNamePath = bindingInfo.Principal.Path.Select(principalVisualIdentityInfo.Name.Path);

    public async ValueTask<bool> HasAccessAsync(CancellationToken cancellationToken) => await this.GetPermissionQuery().GenericAnyAsync(cancellationToken);

    public IAsyncEnumerable<Dictionary<Type, Array>> GetPermissionsAsync(ImmutableArray<Type> securityContextTypes)
    {
        var restrictionFilterInfoList = securityRule.GetSafeSecurityContextRestrictionFilters().ToList();

        return this.GetPermissionQuery(null)
            .GenericAsAsyncEnumerable()
            .Select<TPermission, Dictionary<Type, Array>>(permission => this.ConvertPermission(permission, securityContextTypes, restrictionFilterInfoList));
    }

    public IQueryable<TPermission> GetPermissionQuery() => this.GetPermissionQuery(null);

    private IQueryable<TPermission> GetPermissionQuery(SecurityRuleCredential? customSecurityRuleCredential)
    {
        //TODO: inject SecurityContextRestrictionFilterInfo
        return queryableSource
            .GetQueryable<TPermission>()
            .Where(itemBindingInfo.Filter(serviceProvider))
            .Where(bindingInfo.GetPeriodFilter(timeProvider.GetLocalNow().Date))
            .PipeMaybe(
                defaultCancellationTokenSource.RunSync(ct =>
                    userNameResolver.GetUserNameAsync(customSecurityRuleCredential ?? securityRule.CustomCredential ?? defaultSecurityRuleCredential, ct)),
                (q, principalName) => q.Where(this.fullNamePath.Select(name => name == principalName)));
    }

    public IAsyncEnumerable<string> GetAccessorsAsync(Expression<Func<TPermission, bool>> permissionFilter) =>
        this.GetPermissionQuery(new SecurityRuleCredential.AnyUserCredential())
            .Where(permissionFilter)
            .Select(this.fullNamePath)
            .Distinct()
            .GenericAsAsyncEnumerable();

    private Dictionary<Type, Array> ConvertPermission(
        TPermission permission,
        ImmutableArray<Type> securityContextTypes,
        IReadOnlyCollection<SecurityContextRestrictionFilterInfo> filterInfoList)
    {
        return securityContextTypes.GroupJoin(filterInfoList, sct => sct, f => f.SecurityContextType, (securityContextType, filters) =>
            {
                var pureFilter = filters.SingleOrDefault()?.GetBasePureFilter(serviceProvider);

                var identityInfo = identityInfoSource.GetIdentityInfo(securityContextType);

                var getIdentsArrayExpr = virtualBindingInfo.GetRestrictionsArrayExpr(pureFilter, identityInfo);

                return (securityContextType, this.expressionEvaluator.Evaluate(getIdentsArrayExpr, permission));
            })
            .ToDictionary();
    }
}
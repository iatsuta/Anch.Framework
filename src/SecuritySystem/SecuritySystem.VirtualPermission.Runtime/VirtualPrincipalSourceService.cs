using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using CommonFramework;
using CommonFramework.ExpressionEvaluate;
using CommonFramework.GenericRepository;
using CommonFramework.IdentitySource;
using CommonFramework.VisualIdentitySource;

using GenericQueryable;
using SecuritySystem.ExternalSystem.Management;
using SecuritySystem.Services;
using SecuritySystem.UserSource;

namespace SecuritySystem.VirtualPermission;

public class VirtualPrincipalSourceService<TPrincipal, TPermission>(
    IServiceProvider serviceProvider,
    IExpressionEvaluatorStorage expressionEvaluatorStorage,
    IQueryableSource queryableSource,
    IUserQueryableSource<TPrincipal> userQueryableSource,
    PermissionBindingInfo<TPermission, TPrincipal> bindingInfo,
    VirtualPermissionBindingInfo<TPermission> virtualBindingInfo,
    IIdentityInfoSource identityInfoSource,
    IManagedPrincipalHeaderConverterFactory<TPrincipal> managedPrincipalHeaderConverterFactory,
    ISecurityIdentityManager<TPermission> permissionIdentityManager,
    IVisualIdentityInfo<TPrincipal> principalVisualIdentityInfo) : IPrincipalSourceService

    where TPrincipal : class
    where TPermission : class
{
    private readonly IManagedPrincipalHeaderConverter<TPrincipal> managedPrincipalHeaderConverter =
        managedPrincipalHeaderConverterFactory.Create(bindingInfo);

    private readonly IExpressionEvaluator expressionEvaluator =
        expressionEvaluatorStorage.GetForType(typeof(VirtualPrincipalSourceService<TPrincipal, TPermission>));

    public Type PrincipalType { get; } = typeof(TPrincipal);

    public IAsyncEnumerable<ManagedPrincipalHeader> GetPrincipalsAsync(string nameFilter, int limit) =>

        virtualBindingInfo
            .Items
            .ToAsyncEnumerable()
            .SelectMany(itemBindingInfo =>
                queryableSource
                .GetQueryable<TPermission>()
                .Where(itemBindingInfo.Filter(serviceProvider))
                .Select(bindingInfo.Principal.Path)
                .Where(this.GetNameFilter(nameFilter))
                .OrderBy(principalVisualIdentityInfo.Name.Path)
                .Take(limit)
                .Select(this.managedPrincipalHeaderConverter.ConvertExpression)
                .Distinct()
                .GenericAsAsyncEnumerable())

            .OrderBy(h => h.Name)
            .Distinct()
            .Take(limit);

    private Expression<Func<TPrincipal, bool>> GetNameFilter(string nameFilter) =>

        string.IsNullOrWhiteSpace(nameFilter)
            ? _ => true
            : principalVisualIdentityInfo.Name.Path.Select(principalName => principalName.Contains(nameFilter));

    public async ValueTask<ManagedPrincipal?> TryGetPrincipalAsync(UserCredential userCredential, CancellationToken cancellationToken)
    {
        var principal = await userQueryableSource.GetQueryable(userCredential).GenericSingleOrDefaultAsync(cancellationToken);

        if (principal == null)
        {
            return null;
        }
        else
        {
            var header = this.managedPrincipalHeaderConverter.Convert(principal);

            var managedPermissions = await virtualBindingInfo
                .Items
                .ToAsyncEnumerable()
                .SelectMany(itemBindingInfo => queryableSource.GetQueryable<TPermission>()
                    .Where(itemBindingInfo.Filter(serviceProvider))
                    .Where(bindingInfo.Principal.Path.Select(p => p == principal))
                    .GenericAsAsyncEnumerable()
                    .Select(permission => this.ToManagedPermission(permission, itemBindingInfo.SecurityRole)))
                .ToImmutableArrayAsync(cancellationToken);

            return new ManagedPrincipal(header, managedPermissions);
        }
    }

    private ManagedPermission ToManagedPermission(TPermission permission, SecurityRole securityRole)
    {
        var getRestrictionsMethod = this.GetType().GetMethod(nameof(this.GetRestrictionArray), BindingFlags.Instance | BindingFlags.NonPublic)!;

        var restrictions = virtualBindingInfo
            .SecurityContextTypes
            .Select(identityInfoSource.GetIdentityInfo)
            .ToImmutableDictionary(
                identityInfo => identityInfo.DomainObjectType,
                identityInfo => getRestrictionsMethod
                    .MakeGenericMethod(identityInfo.DomainObjectType, identityInfo.IdentityType)
                    .Invoke<Array>(this, permission, identityInfo));

        return new ManagedPermission
        {
            Identity = permissionIdentityManager.GetIdentity(permission),
            IsVirtual = true,
            SecurityRole = securityRole,
            Period = bindingInfo.GetSafePeriod(permission),
            Comment = bindingInfo.GetSafeComment(permission),
            DelegatedFrom = bindingInfo.DelegatedFrom?.Getter.Invoke(permission) is { } delegatedFromPermission
                ? permissionIdentityManager.GetIdentity(delegatedFromPermission)
                : SecurityIdentity.Default,
            Restrictions = restrictions
        };
    }

    public IAsyncEnumerable<string> GetLinkedPrincipalsAsync(ImmutableHashSet<SecurityRole> securityRoles) =>
        virtualBindingInfo
            .Items
            .ToAsyncEnumerable()
            .Where(itemBindingInfo => securityRoles.Contains(itemBindingInfo.SecurityRole))
            .SelectMany(itemBindingInfo => queryableSource.GetQueryable<TPermission>()
                .Where(itemBindingInfo.Filter(serviceProvider))
                .Select(bindingInfo.Principal.Path)
                .Select(principalVisualIdentityInfo.Name.Path)
                .GenericAsAsyncEnumerable())
            .Distinct();

    private TSecurityContextIdent[] GetRestrictionArray<TSecurityContext, TSecurityContextIdent>(
        TPermission permission,
        IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo)
        where TSecurityContext : ISecurityContext
        where TSecurityContextIdent : notnull =>
        this.GetRestrictionIdents(permission, identityInfo).ToArray();

    private IEnumerable<TSecurityContextIdent> GetRestrictionIdents<TSecurityContext, TSecurityContextIdent>(
        TPermission permission,
        IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo)
        where TSecurityContext : ISecurityContext
        where TSecurityContextIdent : notnull
    {
        foreach (var restrictionPath in virtualBindingInfo.Restrictions)
        {
            if (restrictionPath is Expression<Func<TPermission, TSecurityContext?>> singlePath)
            {
                var securityContext = this.expressionEvaluator.Evaluate(singlePath, permission);

                if (securityContext != null)
                {
                    yield return identityInfo.Id.Getter(securityContext);
                }
            }
            else if (restrictionPath is Expression<Func<TPermission, IEnumerable<TSecurityContext>>> manyPath)
            {
                foreach (var securityContext in this.expressionEvaluator.Evaluate(manyPath, permission))
                {
                    yield return identityInfo.Id.Getter(securityContext);
                }
            }
        }
    }
}
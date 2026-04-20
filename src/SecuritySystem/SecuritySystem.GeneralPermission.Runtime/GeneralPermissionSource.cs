using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using CommonFramework;
using CommonFramework.GenericRepository;
using CommonFramework.IdentitySource;
using CommonFramework.VisualIdentitySource;

using SecuritySystem.ExternalSystem;
using SecuritySystem.Services;

using GenericQueryable;

namespace SecuritySystem.GeneralPermission;

public class GeneralPermissionSource<TPermission>(
    IServiceProxyFactory serviceProxyFactory,
    IIdentityInfo<TPermission> permissionIdentityInfo,
    PermissionBindingInfo<TPermission> bindingInfo,
    IGeneralPermissionRestrictionBindingInfoSource restrictionBindingInfoSource,
    DomainSecurityRule.RoleBaseSecurityRule securityRule) : IPermissionSource<TPermission>
{
    private readonly Lazy<IPermissionSource<TPermission>> lazyInnerService = new(() =>
    {
        var restrictionBindingInfo = restrictionBindingInfoSource.GetForPermission(bindingInfo.PermissionType);

        var innerServiceType = typeof(GeneralPermissionSource<,,,,,>).MakeGenericType(
            bindingInfo.PrincipalType,
            bindingInfo.PermissionType,
            restrictionBindingInfo.PermissionRestrictionType,
            restrictionBindingInfo.SecurityContextTypeType,
            restrictionBindingInfo.SecurityContextObjectIdentType,
            permissionIdentityInfo.IdentityType);

        return serviceProxyFactory.Create<IPermissionSource<TPermission>>(innerServiceType, securityRule);
    });

    private IPermissionSource<TPermission> InnerService => this.lazyInnerService.Value;

    public ValueTask<bool> HasAccessAsync(CancellationToken cancellationToken) => this.InnerService.HasAccessAsync(cancellationToken);

    public IAsyncEnumerable<Dictionary<Type, Array>> GetPermissionsAsync(ImmutableArray<Type> securityContextTypes) =>
        this.InnerService.GetPermissionsAsync(securityContextTypes);

    public IQueryable<TPermission> GetPermissionQuery() => this.InnerService.GetPermissionQuery();

    public IAsyncEnumerable<string> GetAccessorsAsync(Expression<Func<TPermission, bool>> permissionFilter) => this.InnerService.GetAccessorsAsync(permissionFilter);
}

public class GeneralPermissionSource<TPrincipal, TPermission, TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent, TPermissionIdent>(
    PermissionBindingInfo<TPermission, TPrincipal> bindingInfo,
    GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent, TPermission> restrictionBindingInfo,
    IAvailablePermissionSource<TPermission> availablePermissionSource,
    IRawPermissionConverter<TPermissionRestriction> rawPermissionConverter,
    IQueryableSource queryableSource,
    IIdentityInfo<TPermission, TPermissionIdent> permissionIdentityInfo,
    IVisualIdentityInfo<TPrincipal> principalVisualIdentityInfo,
    DomainSecurityRule.RoleBaseSecurityRule securityRule) : IPermissionSource<TPermission>

    where TPrincipal : class
    where TPermission : class
    where TPermissionRestriction : class
    where TSecurityContextType : class
    where TSecurityContextObjectIdent : notnull
    where TPermissionIdent : notnull
{
    public async ValueTask<bool> HasAccessAsync(CancellationToken cancellationToken) => await this.GetPermissionQuery().GenericAnyAsync(cancellationToken);

    public IAsyncEnumerable<Dictionary<Type, Array>> GetPermissionsAsync(ImmutableArray<Type> securityContextTypes) => this.GetPermissionsInternalAsync(securityContextTypes);

    private async IAsyncEnumerable<Dictionary<Type, Array>> GetPermissionsInternalAsync(ImmutableArray<Type> securityContextTypes,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var permissionIdents = await availablePermissionSource
            .GetQueryable(securityRule)
            .Select(permissionIdentityInfo.Id.Path)
            .GenericToArrayAsync(cancellationToken);

        var containsPermissionFilter = permissionIdentityInfo.CreateFilter(permissionIdents);

        var permissionRestrictions = await queryableSource
            .GetQueryable<TPermissionRestriction>()
            .Where(restrictionBindingInfo.Permission.Path.Select(containsPermissionFilter))
            .GenericToArrayAsync(cancellationToken);

        var resultE = permissionIdents.GroupJoin(
            permissionRestrictions, id => id, restrictionBindingInfo.Permission.Getter.Composite(permissionIdentityInfo.Id.Getter),
            (_, restrictions) => rawPermissionConverter.ConvertPermission(securityRule, restrictions, securityContextTypes));

        foreach (var item in resultE)
        {
            yield return item;
        }
    }

    public IQueryable<TPermission> GetPermissionQuery()
    {
        return availablePermissionSource.GetQueryable(securityRule);
    }

    public IAsyncEnumerable<string> GetAccessorsAsync(Expression<Func<TPermission, bool>> permissionFilter)
    {
        var availableFilter = availablePermissionSource.GetQueryable(securityRule with { CustomCredential = new SecurityRuleCredential.AnyUserCredential() });

        return availableFilter
            .Where(permissionFilter)
            .Select(bindingInfo.Principal.Path.Select(principalVisualIdentityInfo.Name.Path))
            .Distinct()
            .GenericAsAsyncEnumerable();
    }
}
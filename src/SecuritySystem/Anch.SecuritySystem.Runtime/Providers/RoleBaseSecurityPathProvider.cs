using Anch.Core;
using Anch.SecuritySystem.Builders._Factory;
using Anch.SecuritySystem.Builders._Filter;
using Anch.SecuritySystem.SecurityAccessor;

namespace Anch.SecuritySystem.Providers;

/// <summary>
/// Контекстный провайдер доступа
/// </summary>
/// <typeparam name="TDomainObject"></typeparam>
public class RoleBaseSecurityPathProvider<TDomainObject>(
    ISecurityFilterFactory<TDomainObject> securityFilterFactory,
    IAccessorsFilterFactory<TDomainObject> accessorsFilterFactory,
    DomainSecurityRule.RoleBaseSecurityRule securityRule,
    SecurityPath<TDomainObject> securityPath,
    IDefaultCancellationTokenSource? defaultCancellationTokenSource = null)
    : ISecurityProvider<TDomainObject>
{
    private SecurityFilterInfo<TDomainObject>? securityFilterCache;

    private AccessorsFilterInfo<TDomainObject>? accessorsFilterCache;

    public IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable) =>
        defaultCancellationTokenSource.RunSync(this.GetOrCreateSecurityFilterInfo).InjectFunc(queryable);

    public async ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken cancellationToken)
    {
        var securityFilterInfo = await this.GetOrCreateSecurityFilterInfo(cancellationToken);

        return securityFilterInfo.HasAccessFunc.Invoke(domainObject);
    }

    public async ValueTask<AccessResult> GetAccessResultAsync(TDomainObject domainObject, CancellationToken cancellationToken)
    {
        if (await this.HasAccessAsync(domainObject, cancellationToken))
        {
            return AccessResult.AccessGrantedResult.Default;
        }
        else
        {
            return AccessResult.AccessDeniedResult.Create(domainObject, securityRule);
        }
    }

    public async ValueTask<SecurityAccessorData> GetAccessorDataAsync(TDomainObject domainObject, CancellationToken cancellationToken)
    {
        var accessorsFilter = this.accessorsFilterCache ??= await accessorsFilterFactory.CreateFilterAsync(securityRule, securityPath, cancellationToken);

        var accessors = await accessorsFilter.GetAccessorsFunc(domainObject).ToImmutableArrayAsync(cancellationToken);

        return new SecurityAccessorData.FixedSecurityAccessorData(accessors);
    }

    private async ValueTask<SecurityFilterInfo<TDomainObject>> GetOrCreateSecurityFilterInfo(CancellationToken cancellationToken) =>

        this.securityFilterCache ??= await securityFilterFactory.CreateFilterAsync(securityRule, securityPath, cancellationToken);
}
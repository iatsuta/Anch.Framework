using CommonFramework;
using GenericQueryable;

using SecuritySystem.SecurityAccessor;

namespace SecuritySystem.Providers;

/// <summary>
/// Провайдер доступа к объектам
/// </summary>
/// <typeparam name="TDomainObject"></typeparam>
public interface ISecurityProvider<TDomainObject> : IQueryableInjector<TDomainObject>
{
    async ValueTask<AccessResult> GetAccessResultAsync(TDomainObject domainObject, CancellationToken cancellationToken = default) =>
        await this.HasAccessAsync(domainObject, cancellationToken)
            ? AccessResult.AccessGrantedResult.Default
            : AccessResult.AccessDeniedResult.Create(domainObject);

    async ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken cancellationToken = default) =>
        await this.Inject(new[] { domainObject }.AsQueryable()).GenericContainsAsync(domainObject, cancellationToken);

    async ValueTask<SecurityAccessorData> GetAccessorDataAsync(TDomainObject domainObject, CancellationToken cancellationToken = default) =>
        await this.HasAccessAsync(domainObject, cancellationToken) ? SecurityAccessorData.Infinity : SecurityAccessorData.Empty;
}
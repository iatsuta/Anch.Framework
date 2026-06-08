using Anch.Core;
using Anch.GenericQueryable;
using Anch.SecuritySystem.SecurityAccessor;

namespace Anch.SecuritySystem.Providers;

/// <summary>
/// Провайдер доступа к объектам
/// </summary>
/// <typeparam name="TDomainObject"></typeparam>
public interface ISecurityProvider<TDomainObject> : IQueryableInjector<TDomainObject>
{
    async ValueTask<AccessResult> GetAccessResultAsync(TDomainObject domainObject, CancellationToken ct) =>
        await this.HasAccessAsync(domainObject, ct)
            ? AccessResult.AccessGrantedResult.Default
            : AccessResult.AccessDeniedResult.Create(domainObject);

    async ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken ct) =>
        await this.Inject(new[] { domainObject }.AsQueryable()).GenericContainsAsync(domainObject, ct);

    async ValueTask<SecurityAccessorData> GetAccessorDataAsync(TDomainObject domainObject, CancellationToken ct) =>
        await this.HasAccessAsync(domainObject, ct) ? SecurityAccessorData.Infinity : SecurityAccessorData.Empty;
}
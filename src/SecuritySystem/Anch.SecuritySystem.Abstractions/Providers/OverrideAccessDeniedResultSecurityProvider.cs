using Anch.SecuritySystem.SecurityAccessor;

namespace Anch.SecuritySystem.Providers;

public class OverrideAccessDeniedResultSecurityProvider<TDomainObject>(
    ISecurityProvider<TDomainObject> baseProvider,
    Func<AccessResult.AccessDeniedResult, AccessResult.AccessDeniedResult> selector)
    : ISecurityProvider<TDomainObject>
{
    public IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable) => baseProvider.Inject(queryable);

    public ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken cancellationToken = default) =>
        baseProvider.HasAccessAsync(domainObject, cancellationToken);

    public ValueTask<SecurityAccessorData> GetAccessorDataAsync(TDomainObject domainObject, CancellationToken cancellationToken = default) =>
        baseProvider.GetAccessorDataAsync(domainObject, cancellationToken);

    public async ValueTask<AccessResult> GetAccessResultAsync(TDomainObject domainObject, CancellationToken cancellationToken) =>
        (await baseProvider.GetAccessResultAsync(domainObject, cancellationToken)).TryOverrideAccessDeniedResult(selector);
}

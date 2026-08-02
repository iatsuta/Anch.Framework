using Anch.SecuritySystem.SecurityAccessor;

namespace Anch.SecuritySystem.Providers;

public class OverrideAccessDeniedResultSecurityProvider<TDomainObject>(
    ISecurityProvider<TDomainObject> baseProvider,
    Func<AccessResult.AccessDeniedResult, AccessResult.AccessDeniedResult> selector)
    : ISecurityProvider<TDomainObject>
{
    public IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable) => baseProvider.Inject(queryable);

    public ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken ct) =>
        baseProvider.HasAccessAsync(domainObject, ct);

    public ValueTask<SecurityAccessorData> GetAccessorDataAsync(TDomainObject domainObject, CancellationToken ct) =>
        baseProvider.GetAccessorDataAsync(domainObject, ct);

    public async ValueTask<AccessResult> GetAccessResultAsync(TDomainObject domainObject, CancellationToken ct) =>
        (await baseProvider.GetAccessResultAsync(domainObject, ct)).TryOverrideAccessDeniedResult(selector);
}

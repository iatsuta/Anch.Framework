using Anch.SecuritySystem.SecurityAccessor;

namespace Anch.SecuritySystem.Providers;

/// <summary>Wraps a provider and rewrites its access denied results, for example to change the message.</summary>
/// <param name="baseProvider">The wrapped provider.</param>
/// <param name="selector">Rewrites a denial.</param>
/// <typeparam name="TDomainObject">The type of the protected domain objects.</typeparam>
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

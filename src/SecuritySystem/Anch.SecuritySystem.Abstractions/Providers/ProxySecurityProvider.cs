using Anch.SecuritySystem.DomainServices;
using Anch.SecuritySystem.SecurityAccessor;

namespace Anch.SecuritySystem.Providers;

public abstract class ProxySecurityProvider<TDomainObject>(IDomainSecurityService<TDomainObject> domainSecurityService, SecurityRule securityRule) : ISecurityProvider<TDomainObject>
{
    private readonly ISecurityProvider<TDomainObject> innerProvider = domainSecurityService.GetSecurityProvider(securityRule);

    public IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable) => this.innerProvider.Inject(queryable);

    public ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken ct) => this.innerProvider.HasAccessAsync(domainObject, ct);

    public ValueTask<AccessResult> GetAccessResultAsync(TDomainObject domainObject, CancellationToken ct) => this.innerProvider.GetAccessResultAsync(domainObject, ct);

    public ValueTask<SecurityAccessorData> GetAccessorDataAsync(TDomainObject domainObject, CancellationToken ct) => this.innerProvider.GetAccessorDataAsync(domainObject, ct);
}
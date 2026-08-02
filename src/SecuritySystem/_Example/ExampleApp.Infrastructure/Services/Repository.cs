using Anch.SecuritySystem.AccessDenied;
using Anch.SecuritySystem.Providers;

using ExampleApp.Application;

namespace ExampleApp.Infrastructure.Services;

public class Repository<TDomainObject>(
    IDal<TDomainObject> dal,
    IAccessDeniedExceptionService accessDeniedExceptionService,
    ISecurityProvider<TDomainObject> securityProvider) : IRepository<TDomainObject>
    where TDomainObject : class
{
    public async Task SaveAsync(TDomainObject domainObject, CancellationToken ct)
    {
        await this.CheckAccess(domainObject, ct);

        await dal.SaveAsync(domainObject, ct);
    }

    public async Task RemoveAsync(TDomainObject domainObject, CancellationToken ct)
    {
        await this.CheckAccess(domainObject, ct);

        await dal.RemoveAsync(domainObject, ct);
    }

    private async Task CheckAccess(TDomainObject domainObject, CancellationToken ct) =>
        await securityProvider.CheckAccessAsync(domainObject, accessDeniedExceptionService, ct);

    public IQueryable<TDomainObject> GetQueryable() => securityProvider.Inject(dal.GetQueryable());
}
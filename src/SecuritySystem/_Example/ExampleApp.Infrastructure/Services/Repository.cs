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
    public async Task SaveAsync(TDomainObject domainObject, CancellationToken cancellationToken)
    {
        await this.CheckAccess(domainObject, cancellationToken);

        await dal.SaveAsync(domainObject, cancellationToken);
    }

    public async Task RemoveAsync(TDomainObject domainObject, CancellationToken cancellationToken)
    {
        await this.CheckAccess(domainObject, cancellationToken);

        await dal.RemoveAsync(domainObject, cancellationToken);
    }

    private Task CheckAccess(TDomainObject domainObject, CancellationToken cancellationToken) =>
        securityProvider.CheckAccessAsync(domainObject, accessDeniedExceptionService, cancellationToken);

    public IQueryable<TDomainObject> GetQueryable() => securityProvider.Inject(dal.GetQueryable());
}
using Anch.GenericRepository;

using Microsoft.Extensions.DependencyInjection;

using NHibernate;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public class NHibGenericRepository(
    IServiceProvider serviceProvider,
    ISession session) : IGenericRepository
{
    public async Task SaveAsync<TDomainObject>(TDomainObject domainObject, CancellationToken ct)
        where TDomainObject : class
    {
        await serviceProvider.GetRequiredService<IDomainObjectSaveStrategy<TDomainObject>>().SaveAsync(session, domainObject, ct);

        await session.FlushAsync(ct);
    }

    public Task RemoveAsync<TDomainObject>(TDomainObject domainObject, CancellationToken ct)
        where TDomainObject : class => session.DeleteAsync(domainObject, ct);
}
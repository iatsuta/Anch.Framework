using Anch.GenericRepository;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.IntegrationTests.Environment;

public class NHibGenericRepository(
    IServiceProvider serviceProvider,
    NHibAutoCommitSession session) : IGenericRepository
{
    public Task SaveAsync<TDomainObject>(TDomainObject domainObject, CancellationToken cancellationToken)
        where TDomainObject : class =>
        serviceProvider.GetRequiredService<IDomainObjectSaveStrategy<TDomainObject>>().SaveAsync(session.NativeSession, domainObject, cancellationToken);


    public Task RemoveAsync<TDomainObject>(TDomainObject domainObject, CancellationToken cancellationToken)
        where TDomainObject : class => session.NativeSession.DeleteAsync(domainObject, cancellationToken);
}
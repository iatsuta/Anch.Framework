using NHibernate;

namespace ExampleApp.Infrastructure.Services;

public interface IDomainObjectSaveStrategy<in TDomainObject>
{
    Task SaveAsync(ISession session, TDomainObject domainObject, CancellationToken ct);
}
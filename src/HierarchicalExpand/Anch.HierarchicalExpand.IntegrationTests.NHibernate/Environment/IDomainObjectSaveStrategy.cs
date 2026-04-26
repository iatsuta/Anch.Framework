using NHibernate;

namespace Anch.HierarchicalExpand.IntegrationTests.Environment;

public interface IDomainObjectSaveStrategy<in TDomainObject>
{
    Task SaveAsync(ISession session, TDomainObject domainObject, CancellationToken cancellationToken);
}
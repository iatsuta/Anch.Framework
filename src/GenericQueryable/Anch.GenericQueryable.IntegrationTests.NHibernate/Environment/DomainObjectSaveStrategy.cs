using Anch.Core;
using Anch.IdentitySource;

using NHibernate;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public class DomainObjectSaveStrategy<TDomainObject>(IServiceProxyFactory serviceProxyFactory) : IDomainObjectSaveStrategy<TDomainObject>
{
    private readonly IDomainObjectSaveStrategy<TDomainObject> proxy = serviceProxyFactory.Create<IDomainObjectSaveStrategy<TDomainObject>>();

    public Task SaveAsync(ISession session, TDomainObject domainObject, CancellationToken ct) =>
        this.proxy.SaveAsync(session, domainObject, ct);
}

public class DomainObjectSaveStrategy<TDomainObject, TIdent>(IIdentityInfo<TDomainObject, TIdent> identityInfo) : IDomainObjectSaveStrategy<TDomainObject>
{
    public Task SaveAsync(ISession session, TDomainObject domainObject, CancellationToken ct)
    {
        if (!session.Contains(domainObject))
        {
            var id = identityInfo.Id.Getter(domainObject);

            if (!EqualityComparer<TIdent>.Default.Equals(id, default))
            {
                return session.SaveAsync(domainObject, id, ct);
            }
        }

        return session.SaveOrUpdateAsync(domainObject, ct);
    }
}
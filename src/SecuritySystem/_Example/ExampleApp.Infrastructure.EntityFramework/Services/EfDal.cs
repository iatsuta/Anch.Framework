using Microsoft.EntityFrameworkCore;

namespace ExampleApp.Infrastructure.Services;

public class EfDal<TDomainObject>(EfAutoCommitSession session) : IDal<TDomainObject>
    where TDomainObject : class
{
    public async Task SaveAsync(TDomainObject domainObject, CancellationToken cancellationToken)
    {
        var state = session.NativeSession.Entry(domainObject).State;

        if (state == EntityState.Detached)
        {
            await session.NativeSession.AddAsync(domainObject, cancellationToken);
        }
    }

    public async Task RemoveAsync(TDomainObject domainObject, CancellationToken cancellationToken)
    {
        session.NativeSession.Remove(domainObject);
    }

    public IQueryable<TDomainObject> GetQueryable()
    {
        return session.NativeSession.Set<TDomainObject>();
    }
}
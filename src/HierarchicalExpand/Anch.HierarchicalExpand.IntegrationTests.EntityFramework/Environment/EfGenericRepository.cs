using Anch.GenericRepository;

using Microsoft.EntityFrameworkCore;

namespace Anch.HierarchicalExpand.IntegrationTests.Environment;

public class EfGenericRepository(EfAutoCommitSession session) : IGenericRepository
{
    public async Task SaveAsync<TDomainObject>(TDomainObject domainObject, CancellationToken ct)
        where TDomainObject : class
    {
        var state = session.NativeSession.Entry(domainObject).State;

        if (state == EntityState.Detached)
        {
            await session.NativeSession.AddAsync(domainObject, ct);
        }
    }

    public async Task RemoveAsync<TDomainObject>(TDomainObject domainObject, CancellationToken ct)
        where TDomainObject : class
    {
        session.NativeSession.Remove(domainObject);
    }
}
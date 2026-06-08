using Anch.GenericRepository;

using Microsoft.EntityFrameworkCore;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public class EfGenericRepository(TestDbContext dbContext) : IGenericRepository
{
    public async Task SaveAsync<TDomainObject>(TDomainObject domainObject, CancellationToken ct)
        where TDomainObject : class
    {
        var state = dbContext.Entry(domainObject).State;

        if (state == EntityState.Detached)
        {
            await dbContext.AddAsync(domainObject, ct);
        }

        await dbContext.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync<TDomainObject>(TDomainObject domainObject, CancellationToken ct)
        where TDomainObject : class
    {
        dbContext.Remove(domainObject);

        await dbContext.SaveChangesAsync(ct);
    }
}
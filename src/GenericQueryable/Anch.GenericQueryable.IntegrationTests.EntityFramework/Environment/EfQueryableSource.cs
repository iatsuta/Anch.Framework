using Anch.GenericRepository;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public class EfQueryableSource(TestDbContext dbContext) : IQueryableSource
{
    public IQueryable<TDomainObject> GetQueryable<TDomainObject>()
        where TDomainObject : class
    {
        return dbContext.Set<TDomainObject>();
    }
}
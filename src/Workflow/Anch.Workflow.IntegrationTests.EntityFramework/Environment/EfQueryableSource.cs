using Anch.GenericRepository;

namespace Anch.Workflow.IntegrationTests.Environment;

public class EfQueryableSource(EfAutoCommitSession session) : IQueryableSource
{
    public IQueryable<TDomainObject> GetQueryable<TDomainObject>()
        where TDomainObject : class
    {
        return session.NativeSession.Set<TDomainObject>();
    }
}
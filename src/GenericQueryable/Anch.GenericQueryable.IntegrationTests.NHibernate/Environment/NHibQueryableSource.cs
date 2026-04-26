using Anch.GenericQueryable.NHibernate;
using Anch.GenericQueryable.Services;
using Anch.GenericRepository;
using NHibernate;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public class NHibQueryableSource(
    ISession session,
    IGenericQueryableExecutor genericQueryableExecutor,
    INHibExpressionVisitorSource? nhibExpressionVisitorSource = null) : IQueryableSource
{
    public IQueryable<TDomainObject> GetQueryable<TDomainObject>()
        where TDomainObject : class
    {
        var queryable = session.Query<TDomainObject>();

        var queryProvider = queryable.Provider as VisitedNHibQueryProvider ??
                            throw new InvalidOperationException($"Register {nameof(VisitedNHibQueryProvider)} in Nhib configuration");

        queryProvider.Visitor = nhibExpressionVisitorSource?.Visitor;
        queryProvider.Executor = genericQueryableExecutor;

        return queryable;
    }
}
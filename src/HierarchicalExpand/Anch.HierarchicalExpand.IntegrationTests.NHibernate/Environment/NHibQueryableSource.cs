using Anch.GenericQueryable.NHibernate;
using Anch.GenericQueryable.Services;
using Anch.GenericRepository;

namespace Anch.HierarchicalExpand.IntegrationTests.Environment;

public class NHibQueryableSource(
    NHibAutoCommitSession session,
    IGenericQueryableExecutor genericQueryableExecutor,
    INHibExpressionVisitorSource? nhibExpressionVisitorSource = null) : IQueryableSource
{
    public IQueryable<TDomainObject> GetQueryable<TDomainObject>()
        where TDomainObject : class
    {
        var queryable = session.NativeSession.Query<TDomainObject>();

        var queryProvider = queryable.Provider as VisitedNHibQueryProvider ??
                            throw new InvalidOperationException($"Register {nameof(VisitedNHibQueryProvider)} in Nhib configuration");

        queryProvider.Visitor = nhibExpressionVisitorSource?.Visitor;
        queryProvider.Executor = genericQueryableExecutor;

        return queryable;
    }
}
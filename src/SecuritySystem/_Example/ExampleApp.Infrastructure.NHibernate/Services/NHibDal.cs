using Anch.GenericQueryable.NHibernate;
using Anch.GenericQueryable.Services;

namespace ExampleApp.Infrastructure.Services;

public class NHibDal<TDomainObject>(
    NHibAutoCommitSession session,
    IDomainObjectSaveStrategy<TDomainObject> saveStrategy,
    IGenericQueryableExecutor genericQueryableExecutor,
    INHibExpressionVisitorSource? nhibExpressionVisitorSource = null) : IDal<TDomainObject>
    where TDomainObject : class
{
    public Task SaveAsync(TDomainObject domainObject, CancellationToken cancellationToken) =>
        saveStrategy.SaveAsync(session.NativeSession, domainObject, cancellationToken);

    public Task RemoveAsync(TDomainObject domainObject, CancellationToken cancellationToken) =>
        session.NativeSession.DeleteAsync(domainObject, cancellationToken);

    public IQueryable<TDomainObject> GetQueryable()
    {
        var queryable = session.NativeSession.Query<TDomainObject>();

        var queryProvider = queryable.Provider as VisitedNHibQueryProvider ??
                            throw new InvalidOperationException($"Register {nameof(VisitedNHibQueryProvider)} in Nhib configuration");

        queryProvider.Visitor = nhibExpressionVisitorSource?.Visitor;
        queryProvider.Executor = genericQueryableExecutor;

        return queryable;
    }
}
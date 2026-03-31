using CommonFramework;
using CommonFramework.GenericRepository;
using CommonFramework.RelativePath;

using SecuritySystem.SecurityAccessor;

namespace SecuritySystem.Providers.DependencySecurity;

public class DependencySecurityProvider<TDomainObject, TBaseDomainObject>(
    ISecurityProvider<TBaseDomainObject> baseSecurityProvider,
    IRelativeDomainPathInfo<TDomainObject, TBaseDomainObject> relativePath,
    IQueryableSource queryableSource)
    : ISecurityProvider<TDomainObject>
    where TBaseDomainObject : class
{
    public IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable)
    {
        var baseDomainObjSecurityQ = queryableSource.GetQueryable<TBaseDomainObject>().Pipe(baseSecurityProvider.Inject);

        return queryable.Where(relativePath.CreateCondition(domainObj => baseDomainObjSecurityQ.Contains(domainObj)));
    }

    public async ValueTask<AccessResult> GetAccessResultAsync(TDomainObject domainObject, CancellationToken cancellationToken)
    {
        var result = await relativePath
            .GetRelativeObjects(domainObject)
            .ToAsyncEnumerable()
            .Select(async (relativeObject, ct) => await baseSecurityProvider.GetAccessResultAsync(relativeObject, ct))
            .ToArrayAsync(cancellationToken);

        return result.Or().TryOverrideDomainObject(domainObject);
    }

    public async ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken cancellationToken) =>
        await relativePath
            .GetRelativeObjects(domainObject)
            .ToAsyncEnumerable()
            .AnyAsync(async (relativeObject, ct) => await baseSecurityProvider.HasAccessAsync(relativeObject, ct), cancellationToken);

    public async ValueTask<SecurityAccessorData> GetAccessorDataAsync(TDomainObject domainObject, CancellationToken cancellationToken)
    {
        var result = await relativePath
            .GetRelativeObjects(domainObject)
            .ToAsyncEnumerable()
            .Select(async (relativeObject, ct) => await baseSecurityProvider.GetAccessorDataAsync(relativeObject, ct))
            .ToArrayAsync(cancellationToken);

        return result.Or();
    }
}

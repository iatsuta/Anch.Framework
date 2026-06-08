using Anch.Core;
using Anch.GenericRepository;
using Anch.RelativePath;
using Anch.SecuritySystem.SecurityAccessor;

namespace Anch.SecuritySystem.Providers.DependencySecurity;

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

    public async ValueTask<AccessResult> GetAccessResultAsync(TDomainObject domainObject, CancellationToken ct)
    {
        var result = await relativePath
            .GetRelativeObjects(domainObject)
            .ToAsyncEnumerable()
            .Select(async (relativeObject, lct) => await baseSecurityProvider.GetAccessResultAsync(relativeObject, lct))
            .ToArrayAsync(ct);

        return result.Or().TryOverrideDomainObject(domainObject);
    }

    public async ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken ct) =>
        await relativePath
            .GetRelativeObjects(domainObject)
            .ToAsyncEnumerable()
            .AnyAsync(async (relativeObject, lct) => await baseSecurityProvider.HasAccessAsync(relativeObject, lct), ct);

    public async ValueTask<SecurityAccessorData> GetAccessorDataAsync(TDomainObject domainObject, CancellationToken ct)
    {
        var result = await relativePath
            .GetRelativeObjects(domainObject)
            .ToAsyncEnumerable()
            .Select(async (relativeObject, lct) => await baseSecurityProvider.GetAccessorDataAsync(relativeObject, lct))
            .ToArrayAsync(ct);

        return result.Or();
    }
}

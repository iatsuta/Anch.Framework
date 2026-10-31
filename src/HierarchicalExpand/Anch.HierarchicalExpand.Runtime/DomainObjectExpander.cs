using Anch.GenericQueryable;
using Anch.GenericRepository;

namespace Anch.HierarchicalExpand;

/// <summary>Walks the tree in memory: ancestors through the parent property, descendants through a cached map of all objects to their parents. The cache lives as long as the expander.</summary>
/// <param name="hierarchicalInfo">The parent property.</param>
/// <param name="queryableSource">Reads all objects when descendants are needed.</param>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
public class DomainObjectExpander<TDomainObject>(HierarchicalInfo<TDomainObject> hierarchicalInfo, IQueryableSource queryableSource)
    : IDomainObjectExpander<TDomainObject>
    where TDomainObject : class
{
    private IReadOnlyDictionary<TDomainObject, TDomainObject?>? baseCache;

    /// <inheritdoc/>
    public ValueTask<HashSet<TDomainObject>> GetAllParents(IEnumerable<TDomainObject> startDomainObjects, CancellationToken ct)
    {
        var allResult = startDomainObjects.ToHashSet();

        for (var nextLayer = allResult; nextLayer.Any(); allResult.UnionWith(nextLayer))
        {
            nextLayer = nextLayer
                .Select(hierarchicalInfo.ParentFunc)
                .Where(nextObj => nextObj is not null && !allResult.Contains(nextObj))
                .Select(nextObj => nextObj!)
                .ToHashSet();
        }

        return new(allResult);
    }

    /// <inheritdoc/>
    public async ValueTask<HashSet<TDomainObject>> GetAllChildren(IEnumerable<TDomainObject> startDomainObjects, CancellationToken ct)
    {
        var allResult = startDomainObjects.ToHashSet();

        if (allResult.Count != 0)
        {
            var cache = await this.GetCache(ct);

            for (var nextLayer = allResult; nextLayer.Any(); allResult.UnionWith(nextLayer))
            {
                nextLayer = cache.Where(pair => pair.Value is not null && nextLayer.Contains(pair.Value))
                    .Select(pair => pair.Key)
                    .ToHashSet();
            }
        }

        return allResult;
    }

    private async Task<IReadOnlyDictionary<TDomainObject, TDomainObject?>> GetCache(CancellationToken ct) =>
        this.baseCache ??= await queryableSource.GetQueryable<TDomainObject>().WithFetch(r => r.Fetch(hierarchicalInfo.ParentPath))
            .GenericToDictionaryAsync(d => d, hierarchicalInfo.ParentFunc, ct);
}
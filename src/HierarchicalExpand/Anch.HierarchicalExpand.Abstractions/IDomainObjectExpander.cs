namespace Anch.HierarchicalExpand;

public interface IDomainObjectExpander<TDomainObject>
    where TDomainObject : class
{
    ValueTask<HashSet<TDomainObject>> GetAllParents(IEnumerable<TDomainObject> startDomainObjects, CancellationToken ct);

    ValueTask<HashSet<TDomainObject>> GetAllChildren(IEnumerable<TDomainObject> startDomainObjects, CancellationToken ct);
}
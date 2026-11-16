namespace Anch.HierarchicalExpand;

/// <summary>Expands domain objects to their ancestors or descendants in the tree (works with objects, not identifiers).</summary>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
public interface IDomainObjectExpander<TDomainObject>
    where TDomainObject : class
{
    /// <summary>Returns the start objects and all their ancestors.</summary>
    /// <param name="startDomainObjects">The objects to start from.</param>
    /// <param name="ct">Cancellation token.</param>
    ValueTask<HashSet<TDomainObject>> GetAllParents(IEnumerable<TDomainObject> startDomainObjects, CancellationToken ct);

    /// <summary>Returns the start objects and all their descendants.</summary>
    /// <param name="startDomainObjects">The objects to start from.</param>
    /// <param name="ct">Cancellation token.</param>
    ValueTask<HashSet<TDomainObject>> GetAllChildren(IEnumerable<TDomainObject> startDomainObjects, CancellationToken ct);
}
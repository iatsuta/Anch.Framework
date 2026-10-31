namespace Anch.HierarchicalExpand.Denormalization;

/// <summary>Computes the changes of the directed ancestor links.</summary>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
/// <typeparam name="TDirectAncestorLink">The directed link entity type.</typeparam>
public interface IAncestorLinkExtractor<TDomainObject, TDirectAncestorLink>
{
    /// <summary>Computes the changes needed to make all links consistent with the whole tree.</summary>
    /// <param name="ct">Cancellation token.</param>
    Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncAllResult(CancellationToken ct);

    /// <summary>Computes the changes for changed and removed objects.</summary>
    /// <param name="updatedDomainObjectsBase">Objects that were added or whose parent changed.</param>
    /// <param name="removedDomainObjects">Objects that were deleted.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncResult(
        IEnumerable<TDomainObject> updatedDomainObjectsBase,
        IEnumerable<TDomainObject> removedDomainObjects,
        CancellationToken ct);

    /// <summary>Computes the changes for one object.</summary>
    /// <param name="domainObject">The changed object.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncResult(TDomainObject domainObject, CancellationToken ct);
}
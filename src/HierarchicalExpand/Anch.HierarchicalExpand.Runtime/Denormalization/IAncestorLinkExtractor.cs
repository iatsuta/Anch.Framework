namespace Anch.HierarchicalExpand.Denormalization;

public interface IAncestorLinkExtractor<TDomainObject, TDirectAncestorLink>
{
    Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncAllResult(CancellationToken ct);

    Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncResult(
        IEnumerable<TDomainObject> updatedDomainObjectsBase,
        IEnumerable<TDomainObject> removedDomainObjects,
        CancellationToken ct);

    Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncResult(TDomainObject domainObject, CancellationToken ct);
}
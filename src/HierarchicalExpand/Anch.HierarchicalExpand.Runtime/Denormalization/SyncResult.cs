using Anch.Core;

namespace Anch.HierarchicalExpand.Denormalization;

/// <summary>The changes to apply to the stored ancestor links.</summary>
/// <param name="Adding">Links to create.</param>
/// <param name="Removing">Link entities to delete.</param>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
/// <typeparam name="TDirectAncestorLink">The directed link entity type.</typeparam>
public record SyncResult<TDomainObject, TDirectAncestorLink>(
    DeepEqualsCollection<AncestorLinkData<TDomainObject>> Adding,
    DeepEqualsCollection<TDirectAncestorLink> Removing)
{
    /// <summary>Creates the result from sequences.</summary>
    /// <param name="adding">Links to create.</param>
    /// <param name="removing">Link entities to delete.</param>
    public SyncResult(IEnumerable<AncestorLinkData<TDomainObject>> adding,
        IEnumerable<TDirectAncestorLink> removing) :
        this(DeepEqualsCollection.Create(adding), DeepEqualsCollection.Create(removing))
    {
    }

    /// <summary>Joins two results.</summary>
    /// <param name="other">The other result.</param>
    public SyncResult<TDomainObject, TDirectAncestorLink> Union(SyncResult<TDomainObject, TDirectAncestorLink> other)
    {
        return new SyncResult<TDomainObject, TDirectAncestorLink>(this.Adding.Union(other.Adding).ToArray(), this.Removing.Union(other.Removing).ToArray());
    }

    /// <summary>A result with no changes.</summary>
    public static SyncResult<TDomainObject, TDirectAncestorLink> Empty { get; } = new([], []);
}
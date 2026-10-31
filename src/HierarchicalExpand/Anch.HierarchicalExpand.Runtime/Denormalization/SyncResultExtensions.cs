namespace Anch.HierarchicalExpand.Denormalization;

/// <summary>Extensions for <see cref="SyncResult{TDomainObject, TDirectAncestorLink}"/>.</summary>
public static class SyncResultExtensions
{
    /// <summary>Joins all results into one.</summary>
    /// <param name="source">The results.</param>
    public static SyncResult<TDomainObject, TDirectAncestorLink> Aggregate<TDomainObject, TDirectAncestorLink>(
        this IEnumerable<SyncResult<TDomainObject, TDirectAncestorLink>> source)
    {
        return source.Aggregate(SyncResult<TDomainObject, TDirectAncestorLink>.Empty, (s1, s2) => s1.Union(s2));
    }
}
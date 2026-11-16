using Anch.Core;

namespace Anch.HierarchicalExpand.Denormalization;

/// <summary>Keeps the stored ancestor links up to date; <c>Initialize</c> rebuilds them for all objects.</summary>
public interface IAncestorDenormalizer : IInitializer;

/// <summary>Keeps the stored ancestor links of <typeparamref name="TDomainObject"/> consistent with its tree.</summary>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
public interface IAncestorDenormalizer<in TDomainObject> : IAncestorDenormalizer
{
    /// <summary>Recomputes the ancestor links for changed and removed objects.</summary>
    /// <param name="updatedDomainObjectsBase">Objects that were added or whose parent changed.</param>
    /// <param name="removedDomainObjects">Objects that were deleted.</param>
    /// <param name="ct">Cancellation token.</param>
    Task SyncAsync(
        IEnumerable<TDomainObject> updatedDomainObjectsBase,
        IEnumerable<TDomainObject> removedDomainObjects,
        CancellationToken ct);
}
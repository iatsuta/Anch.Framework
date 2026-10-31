using Anch.Core;

namespace Anch.HierarchicalExpand.Denormalization;

/// <summary>Keeps the stored depth values up to date; <c>Initialize</c> recomputes them for all objects.</summary>
public interface IDeepLevelDenormalizer : IInitializer;

/// <summary>Keeps the stored depth (<see cref="DeepLevelInfo{TDomainObject}"/>) of <typeparamref name="TDomainObject"/> consistent with its tree.</summary>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
public interface IDeepLevelDenormalizer<in TDomainObject> : IDeepLevelDenormalizer
{
    /// <summary>Recomputes the depth of the given objects.</summary>
    /// <param name="domainObjects">Objects whose depth must be updated.</param>
    /// <param name="ct">Cancellation token.</param>
    Task UpdateDeepLevels(IEnumerable<TDomainObject> domainObjects, CancellationToken ct);
}
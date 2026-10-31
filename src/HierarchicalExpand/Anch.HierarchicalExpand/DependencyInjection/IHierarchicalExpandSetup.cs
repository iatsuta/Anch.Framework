using System.Linq.Expressions;

namespace Anch.HierarchicalExpand.DependencyInjection;

/// <summary>Fluent configuration of the HierarchicalExpand module.</summary>
public interface IHierarchicalExpandSetup
{
    /// <summary>Registers the hierarchy description of a domain type.</summary>
    /// <param name="hierarchicalInfo">The parent property.</param>
    /// <param name="fullAncestorLinkInfo">The directed and undirected ancestor links.</param>
    /// <param name="deepLevelInfo">The stored depth property; <c>null</c> if the type has none.</param>
    /// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
    IHierarchicalExpandSetup AddHierarchicalInfo<TDomainObject>(
        HierarchicalInfo<TDomainObject> hierarchicalInfo,
        FullAncestorLinkInfo<TDomainObject> fullAncestorLinkInfo,
        DeepLevelInfo<TDomainObject>? deepLevelInfo = null);

    /// <summary>Registers the hierarchy description of a domain type from property expressions.</summary>
    /// <param name="parentPath">Selects the parent of an object.</param>
    /// <param name="directed">The directed ancestor links (ancestor to descendant).</param>
    /// <param name="undirected">The undirected ancestor links.</param>
    /// <param name="deepLevelPath">Selects the stored depth property; <c>null</c> if the type has none.</param>
    /// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
    /// <typeparam name="TDirectedLink">The directed link entity type.</typeparam>
    /// <typeparam name="TUndirectedLink">The undirected link entity type.</typeparam>
    IHierarchicalExpandSetup AddHierarchicalInfo<TDomainObject, TDirectedLink, TUndirectedLink>(
        Expression<Func<TDomainObject, TDomainObject?>> parentPath,
        AncestorLinkInfo<TDomainObject, TDirectedLink> directed,
        AncestorLinkInfo<TDomainObject, TUndirectedLink> undirected,
        Expression<Func<TDomainObject, int>>? deepLevelPath = null) =>
        this.AddHierarchicalInfo(
            new HierarchicalInfo<TDomainObject>(parentPath),
            new FullAncestorLinkInfo<TDomainObject, TDirectedLink, TUndirectedLink>(directed, undirected),
            deepLevelPath is null ? null : new DeepLevelInfo<TDomainObject>(deepLevelPath));
}
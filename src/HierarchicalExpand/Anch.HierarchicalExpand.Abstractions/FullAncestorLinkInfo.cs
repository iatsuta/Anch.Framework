namespace Anch.HierarchicalExpand;

/// <summary>
/// Both ancestor link sets of a hierarchical type: the directed links (ancestor to descendant)
/// and the undirected links (both directions, usually a database view).
/// </summary>
/// <param name="Directed">Links from an ancestor to each of its descendants.</param>
/// <param name="Undirected">Links that connect objects in both directions.</param>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
/// <typeparam name="TDirectedLink">The directed link entity type.</typeparam>
/// <typeparam name="TUndirectedLink">The undirected link entity type.</typeparam>
public record FullAncestorLinkInfo<TDomainObject, TDirectedLink, TUndirectedLink>(
    AncestorLinkInfo<TDomainObject, TDirectedLink> Directed,
    AncestorLinkInfo<TDomainObject, TUndirectedLink> Undirected) : FullAncestorLinkInfo<TDomainObject, TDirectedLink>(Directed)
{
    public override Type UndirectedLinkType { get; } = typeof(TUndirectedLink);
}

/// <summary>Typed description that knows the directed link set.</summary>
/// <param name="Directed">Links from an ancestor to each of its descendants.</param>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
/// <typeparam name="TDirectedLink">The directed link entity type.</typeparam>
public abstract record FullAncestorLinkInfo<TDomainObject, TDirectedLink>(AncestorLinkInfo<TDomainObject, TDirectedLink> Directed) : FullAncestorLinkInfo<TDomainObject>
{
    public override Type DirectedLinkType { get; } = typeof(TDirectedLink);
}

/// <summary>Typed description that knows the domain type.</summary>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
public abstract record FullAncestorLinkInfo<TDomainObject> : FullAncestorLinkInfo
{
    public override Type DomainObjectType { get; } = typeof(TDomainObject);
}

/// <summary>Untyped description of the ancestor link sets of a hierarchical type.</summary>
public abstract record FullAncestorLinkInfo
{
    /// <summary>The hierarchical domain type.</summary>
    public abstract Type DomainObjectType { get; }

    /// <summary>The entity type of the directed links.</summary>
    public abstract Type DirectedLinkType { get; }

    /// <summary>The entity type of the undirected links.</summary>
    public abstract Type UndirectedLinkType { get; }
}
namespace Anch.HierarchicalExpand.Denormalization;

/// <summary>A link to create: an ancestor and one of its descendants.</summary>
/// <param name="Ancestor">The ancestor.</param>
/// <param name="Child">The descendant.</param>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
public record AncestorLinkData<TDomainObject>(TDomainObject Ancestor, TDomainObject Child);
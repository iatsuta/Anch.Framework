namespace Anch.HierarchicalExpand;

/// <summary>Creates identifier expanders for domain types. A non-hierarchical type gets an expander that returns its input unchanged.</summary>
public interface IHierarchicalObjectExpanderFactory
{
    /// <summary>Creates the expander for a domain type.</summary>
    /// <param name="domainType">The domain type to expand.</param>
    IHierarchicalObjectExpander Create(Type domainType);
}
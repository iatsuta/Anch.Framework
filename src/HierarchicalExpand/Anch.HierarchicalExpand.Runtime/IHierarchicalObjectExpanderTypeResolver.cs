namespace Anch.HierarchicalExpand;

/// <summary>Chooses the identifier expander implementation for a domain type.</summary>
public interface IHierarchicalObjectExpanderTypeResolver
{
    /// <summary>Returns the expander type.</summary>
    /// <param name="domainType">The domain type.</param>
    Type Resolve(Type domainType);
}
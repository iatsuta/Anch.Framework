namespace Anch.HierarchicalExpand;

/// <summary>Provides the registered hierarchy descriptions of domain types.</summary>
public interface IHierarchicalInfoSource
{
    /// <summary>Returns the hierarchy description of <typeparamref name="TDomainObject"/>.</summary>
    /// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
    HierarchicalInfo<TDomainObject> GetHierarchicalInfo<TDomainObject>();

    /// <summary>Determines whether a hierarchy description is registered for the type.</summary>
    /// <param name="domainType">The domain type to check.</param>
    bool IsHierarchical(Type domainType);
}
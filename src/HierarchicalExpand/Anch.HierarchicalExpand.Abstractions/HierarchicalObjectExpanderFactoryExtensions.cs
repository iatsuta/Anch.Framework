namespace Anch.HierarchicalExpand;

/// <summary>Extensions for <see cref="IHierarchicalObjectExpanderFactory"/>.</summary>
public static class HierarchicalObjectExpanderFactoryExtensions
{
    /// <summary>Creates the expander for <paramref name="domainType"/> and casts it to the typed interface.</summary>
    /// <param name="hierarchicalObjectExpanderFactory">The factory.</param>
    /// <param name="domainType">The domain type to expand.</param>
    /// <typeparam name="TIdent">Type of the identifier of the domain type.</typeparam>
    public static IHierarchicalObjectExpander<TIdent> Create<TIdent>(
        this IHierarchicalObjectExpanderFactory hierarchicalObjectExpanderFactory, Type domainType)
        where TIdent : notnull
    {
        return (IHierarchicalObjectExpander<TIdent>)hierarchicalObjectExpanderFactory.Create(domainType);
    }
}
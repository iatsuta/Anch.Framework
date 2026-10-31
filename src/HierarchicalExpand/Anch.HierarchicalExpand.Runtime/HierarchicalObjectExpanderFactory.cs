using System.Collections.Concurrent;

using Anch.Core;

namespace Anch.HierarchicalExpand;

/// <summary>Creates identifier expanders: chooses the implementation type with the resolver, creates it through the service proxy factory and caches it per domain type.</summary>
/// <param name="serviceProxyFactory">Creates the expander.</param>
/// <param name="hierarchicalObjectExpanderTypeResolver">Chooses the implementation type.</param>
public class HierarchicalObjectExpanderFactory(
    IServiceProxyFactory serviceProxyFactory,
    IHierarchicalObjectExpanderTypeResolver hierarchicalObjectExpanderTypeResolver)
    : IHierarchicalObjectExpanderFactory
{
    private readonly ConcurrentDictionary<Type, IHierarchicalObjectExpander> cache = [];

    /// <inheritdoc/>
    public IHierarchicalObjectExpander Create(Type domainType) => this.cache.GetOrAdd(domainType, _ => serviceProxyFactory.Create<IHierarchicalObjectExpander>(hierarchicalObjectExpanderTypeResolver.Resolve(domainType)));
}
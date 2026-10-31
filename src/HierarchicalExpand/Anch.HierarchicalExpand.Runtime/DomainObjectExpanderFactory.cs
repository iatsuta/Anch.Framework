using Anch.Core;

namespace Anch.HierarchicalExpand;

/// <summary>Creates the default <see cref="DomainObjectExpander{TDomainObject}"/> through the service proxy factory, so the implementation can be redirected.</summary>
/// <param name="serviceProxyFactory">Creates the expander.</param>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
public class DomainObjectExpanderFactory<TDomainObject>(IServiceProxyFactory serviceProxyFactory) : IDomainObjectExpanderFactory<TDomainObject>
    where TDomainObject : class
{
    /// <inheritdoc/>
    public IDomainObjectExpander<TDomainObject> Create()
    {
        return serviceProxyFactory.Create<IDomainObjectExpander<TDomainObject>, DomainObjectExpander<TDomainObject>>();
    }
}
namespace Anch.HierarchicalExpand;

/// <summary>Creates the <see cref="IDomainObjectExpander{TDomainObject}"/> for a domain type.</summary>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
public interface IDomainObjectExpanderFactory<TDomainObject>
    where TDomainObject : class
{
    /// <summary>Creates the expander.</summary>
    IDomainObjectExpander<TDomainObject> Create();
}
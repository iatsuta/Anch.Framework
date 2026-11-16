using System.Linq.Expressions;

namespace Anch.HierarchicalExpand;

/// <summary>Describes how objects of <typeparamref name="TDomainObject"/> form a tree: the property that references the parent.</summary>
/// <param name="ParentPath">Selects the parent of an object; <c>null</c> for a root.</param>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
public record HierarchicalInfo<TDomainObject>(Expression<Func<TDomainObject, TDomainObject?>> ParentPath) : HierarchicalInfo
{
    /// <summary>The compiled <see cref="ParentPath"/>.</summary>
    public Func<TDomainObject, TDomainObject?> ParentFunc { get; } = ParentPath.Compile();

    /// <inheritdoc/>
    public override Type DomainObjectType { get; } = typeof(TDomainObject);
}

/// <summary>Untyped description of the tree structure of a hierarchical type.</summary>
public abstract record HierarchicalInfo
{
    /// <summary>The hierarchical domain type.</summary>
    public abstract Type DomainObjectType { get; }
}
using System.Linq.Expressions;

using Anch.Core;

namespace Anch.HierarchicalExpand;

/// <summary>Untyped description of the stored "depth in the tree" property of a hierarchical type.</summary>
public abstract record DeepLevelInfo
{
    /// <summary>The hierarchical domain type.</summary>
    public abstract Type DomainObjectType { get; }
}

/// <summary>Describes the stored "depth in the tree" property (the root has the smallest value) of <typeparamref name="TDomainObject"/>.</summary>
/// <param name="DeepLevel">Accessors of the depth property.</param>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
public record DeepLevelInfo<TDomainObject>(PropertyAccessors<TDomainObject, int> DeepLevel) : DeepLevelInfo
{
    /// <summary>Creates the description from a property expression.</summary>
    /// <param name="deepLevelPath">Selects the depth property.</param>
    public DeepLevelInfo(Expression<Func<TDomainObject, int>> deepLevelPath)
        : this(deepLevelPath.ToPropertyAccessors())
    {
    }

    /// <inheritdoc/>
    public override Type DomainObjectType { get; } = typeof(TDomainObject);
}
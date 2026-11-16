using System.Linq.Expressions;

using Anch.Core;

namespace Anch.HierarchicalExpand;

/// <summary>
/// Describes an "ancestor link" entity: a stored row that links one domain object to another one in the same hierarchy
/// (for example, an ancestor and one of its descendants).
/// </summary>
/// <param name="From">The link property that references the source object.</param>
/// <param name="To">The link property that references the target object.</param>
/// <typeparam name="TDomainObject">The hierarchical domain type.</typeparam>
/// <typeparam name="TAncestorLink">The link entity type.</typeparam>
public record AncestorLinkInfo<TDomainObject, TAncestorLink>(
    PropertyAccessors<TAncestorLink, TDomainObject> From,
    PropertyAccessors<TAncestorLink, TDomainObject> To)
{
    /// <summary>Creates the description from property expressions.</summary>
    /// <param name="fromPath">Selects the source object of the link.</param>
    /// <param name="toPath">Selects the target object of the link.</param>
    public AncestorLinkInfo(
        Expression<Func<TAncestorLink, TDomainObject>> fromPath,
        Expression<Func<TAncestorLink, TDomainObject>> toPath)
        : this(fromPath.ToPropertyAccessors(), toPath.ToPropertyAccessors())
    {
    }

    /// <summary>Returns the description with <c>From</c> and <c>To</c> swapped, i.e. the link read in the opposite direction.</summary>
    public AncestorLinkInfo<TDomainObject, TAncestorLink> Reverse() => new(this.To, this.From);
}
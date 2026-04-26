using System.Linq.Expressions;

using Anch.Core;

namespace Anch.HierarchicalExpand;

public record AncestorLinkInfo<TDomainObject, TAncestorLink>(
    PropertyAccessors<TAncestorLink, TDomainObject> From,
    PropertyAccessors<TAncestorLink, TDomainObject> To)
{
    public AncestorLinkInfo(
        Expression<Func<TAncestorLink, TDomainObject>> fromPath,
        Expression<Func<TAncestorLink, TDomainObject>> toPath)
        : this(fromPath.ToPropertyAccessors(), toPath.ToPropertyAccessors())
    {
    }

    public AncestorLinkInfo<TDomainObject, TAncestorLink> Reverse() => new(this.To, this.From);
}
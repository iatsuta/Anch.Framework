using System.Linq.Expressions;
using Anch.Core;

namespace Anch.HierarchicalExpand;

public abstract record DeepLevelInfo
{
    public abstract Type DomainObjectType { get; }
}

public record DeepLevelInfo<TDomainObject>(PropertyAccessors<TDomainObject, int> DeepLevel) : DeepLevelInfo
{
    public DeepLevelInfo(Expression<Func<TDomainObject, int>> deepLevelPath)
        : this(deepLevelPath.ToPropertyAccessors())
    {
    }

    public override Type DomainObjectType { get; } = typeof(TDomainObject);
}